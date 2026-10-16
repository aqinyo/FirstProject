using FirstProject.Data;
using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Model.Vo;
using FirstProject.Model.VO;
using FirstProject.MyException;
using FirstProject.Repository;
using FirstProject.Result;          // 注意公司规范：三级以后的命名空间 / 自定义的命名空间引用语句，写在最后面
using FirstProject.Utils;
using FirstProject.Utils.Auth;  
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Org.BouncyCastle.Crypto;
using StackExchange.Redis;
using System.Security.Principal;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FirstProject.Service
{
    /// <summary>
    /// 用户管理实现类
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserRepository m_userRepository;    // 依赖注入DL层的方法（操作数据库）
        private readonly JwtToken m_jwtToken;                // 依赖注入自定义的 JwtToken 工具类，用于生成 JWT Token
        private readonly Sm4 m_sm4;                          // 依赖注入自定义的 Sm4 工具类，用于密码加密和解密
        private readonly IDistributedCache m_distributedCache; // 依赖注入 Redis
        /// <summary>
        /// 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="sm4">SM4密码加密</param>
        /// <param name="distributedCache">Redis 缓存</param>
        public UserService(UserRepository userRepository, JwtToken jwtToken, Sm4 sm4, IDistributedCache distributedCache)
        {
            m_userRepository = userRepository;
            m_jwtToken = jwtToken;
            m_sm4 = sm4;
            m_distributedCache = distributedCache;
        }



        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        /// <exception cref="Exception"></exception>
        public void Register(RegisterDto registerDto)
        {
            try
            {
                // 先判断账号是否已经存在（直接操作数据库）
                bool accountIsExist = m_userRepository.AccountExists(registerDto.Account);        // Any() 方法来判断记录是否存在（它会返回一个布尔值，表示是否存在满足条件的记录）
                if (accountIsExist)                                                              // if()里面的值刚好也是布尔值，进行判断的
                {
                    // 抛出自定义异常 BusinessException 由全局异常处理器来捕获处理  （参数是：业务码、异常提示信息）    （HTTP状态码通过“全局异常处理器”去转换业务码为 HTTP 状态码）
                    throw new BusinessException(BusinessErrorCode.AccountAlreadyExists, ConstantMessage.AccountAlreadyExists);
                }

                // 自己new一个实体类User出来，自己赋值（改变了原本的做法：对象属性映射 + 手动赋值）
                User userRegister = new User();
                // 手动赋值
                userRegister.Name = registerDto.Name;                       // 把入参的 registerDto 中的参数提出来赋值给自己 new 的实体类
                userRegister.Account = registerDto.Account;
                // 前端已用 SM4 加密传输：直接存储前端传来的密文，不要再 Encrypt（否则会双重加密导致登录失败）
                userRegister.Password = registerDto.Password;
                userRegister.Role = "User";                                 // 默认注册的用户角色为普通用户
                userRegister.Status = AccountStatus.Enabled;                // 默认注册的用户状态为启用
                userRegister.CreateTime = DateTime.Now;

                m_userRepository.Register(userRegister);
            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public LoginVo Login(LoginDto loginDto)
        {
            try
            {
                //调用 Repository 层的登录方法进行判断
                User? user = m_userRepository.Login(loginDto);     

                if (user is null)
                {
                    throw new BusinessException(BusinessErrorCode.InvalidCredentials, ConstantMessage.InvalidCredentials);
                }
                if (user.Status != AccountStatus.Enabled)
                {
                    throw new BusinessException(BusinessErrorCode.AccountDisabled, ConstantMessage.AccountDisabled);
                }

                // 前端已用 SM4 加密传输：先解密得到明文，再让 Verify 与存储密文比对
                string password;
                try
                {
                    password = m_sm4.Decrypt(loginDto.Password);
                }
                catch (FormatException)
                {
                    throw new BusinessException(BusinessErrorCode.InvalidCredentials, ConstantMessage.InvalidCredentials);
                }
                catch (InvalidCipherTextException)
                {
                    throw new BusinessException(BusinessErrorCode.InvalidCredentials, ConstantMessage.InvalidCredentials);
                }

                if (!m_sm4.Verify(password, user.Password)) //调用自定义的 Sm4 工具类的 Verify 方法，验证用户输入的密码是否与数据库中存储的加密密码匹配
                {
                    throw new BusinessException(BusinessErrorCode.InvalidCredentials, ConstantMessage.InvalidCredentials);
                }

                // 引入了Redis的Guid
                string jti = Guid.NewGuid().ToString("N");
                DateTime expiresAt = DateTime.UtcNow.AddHours(1);

                // 生成token
                string token = m_jwtToken.CreateToken(user, jti, expiresAt);

                // 登录成功后预热用户资料缓存（key的规则：使用 userId 作为 key）
                UserVo userVo = user.Adapt<UserVo>();
                try
                {
                    m_distributedCache.SetString(
                        GetUserProfileCacheKey(user.Id),
                        JsonSerializer.Serialize(userVo),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpiration = expiresAt
                        });
                }
                catch
                {
                    // 写日志
                }

                LoginVo loginVo = new LoginVo();
                loginVo.AccessToken = token;

                return loginVo;

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 根据用户 ID 获取用户资料，优先从 Redis 缓存读取
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserVo? GetUserProfile(int userId)
        {
            try
            {
                // 先查 Redis，没有数据，再查询数据库
                string cacheKey = GetUserProfileCacheKey(userId);
                try
                {
                    string? cachedUserJson = m_distributedCache.GetString(cacheKey);
                    if (!string.IsNullOrWhiteSpace(cachedUserJson))
                    {
                        UserVo? cachedUser = JsonSerializer.Deserialize<UserVo>(cachedUserJson);
                        if (cachedUser?.Id == userId)
                        {
                            return cachedUser;
                        }
                    }
                }
                catch
                {
                    // Redis 读取失败时继续回源数据库。
                }

                User? user = m_userRepository.GetUserById(userId);
                if (user is null || user.Status != AccountStatus.Enabled)
                {
                    return null;
                }

                UserVo userVo = user.Adapt<UserVo>();
                try
                {
                    m_distributedCache.SetString(
                        cacheKey,
                        JsonSerializer.Serialize(userVo),
                        new DistributedCacheEntryOptions
                        {
                            // 缓存 1小时
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                        });
                }
                catch
                {
                    // 缓存写入失败时仍返回数据库查询结果。
                }

                return userVo;
            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 统一生成用户资料缓存 key；Program.cs 的 InstanceName 会再加上 FirstProject: 前缀。
        /// </summary>
        private static string GetUserProfileCacheKey(int userId)
        {
            return $"user:profile:{userId}";
        }

        /// <summary>
        /// 删除用户资料缓存，避免数据库资料变更后继续返回旧缓存。
        /// </summary>
        private void RemoveUserProfileCache(int userId)
        {
            try
            {
                m_distributedCache.Remove(GetUserProfileCacheKey(userId));
            }
            catch
            {
                // 缓存清理失败时由过期时间兜底。
            }
        }



        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns>将查询到的 User 实体对象映射为 UserVo 对象返回 (因为密码要屏蔽掉)</returns>
        public UserVo? GetUserById(int id)
        {
            try
            {
                // try 里面执行接口实现类方法的业务逻辑
                User? user = m_userRepository.GetUserById(id);  // DL层可能返回空的实体类

                if (user == null)
                {
                    return null;
                }

                // new 出来手动赋值
                UserVo userVo = new UserVo();
                userVo.Id = user.Id;
                userVo.Name = user.Name;
                userVo.Account = user.Account;
                userVo.Role = user.Role;
                userVo.Status = user.Status;
                userVo.CreateTime = user.CreateTime;

                return userVo;

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }

        }



        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        /// <param name="pageQueryDto"></param>
        /// <returns></returns>
        public PageResult<UserVo> GetUserPageList(PageQueryDto pageQueryDto)
        {
            try
            {
                // 1. 调用DL层的数据库操作查询获取数据
                PageResult<User> userPage = m_userRepository.GetUserPageList(pageQueryDto);

                // 2. 将 上面的分页结果集类 手动赋值为 Vo 类
                PageResult<UserVo> userVoPage = new PageResult<UserVo>();
                userVoPage.Total = userPage.Total;
                userVoPage.Page = userPage.Page;
                userVoPage.PageSize = userPage.PageSize;
                userVoPage.Data = userPage.Data?.Adapt<List<UserVo>>();

                // 3. 返回新的 PageResult<UserVo> 给Controller层
                return userVoPage;

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 动态多条件查询用户（单个/多个）（按ID/名称/账号/状态 查询）
        /// </summary>
        /// <param name="queryDto">账号、名称、状态</param>
        /// <returns></returns>
        public PageResult<UserVo> GetUserPageQueryList(UserQueryDto queryDto)
        {
            try
            {
                PageResult<User> userPage = m_userRepository.GetUserPageQueryList(queryDto);
                
                // new 出来手动赋值
                PageResult<UserVo> userVoPage = new PageResult<UserVo>();
                userVoPage.Total = userPage.Total;
                userVoPage.Page = userPage.Page;
                userVoPage.PageSize = userPage.PageSize;
                userVoPage.Data = userPage.Data?.Adapt<List<UserVo>>();

                return userVoPage;

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }


        
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="addUserDto"></param>
        public void AddUser(AddUserDto addUserDto)
        {
            try
            {
                // 手动赋值
                User user = new User();
                user.Name = addUserDto.Name;
                user.Account = addUserDto.Account;
                user.Role = addUserDto.Role;
                user.Status = addUserDto.Status;

                // 密码赋值前先判断是否Null
                if (!string.IsNullOrEmpty(addUserDto.Password))
                {
                    user.Password = addUserDto.Password;   //前端已用 SM4 加密传输：直接存储，不用再调 sm4 的 Encrypt 加密方法
                }
                user.CreateTime = DateTime.Now;

                m_userRepository.AddUser(user);

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="dto"></param>
        public bool UpdateUser(UpdateUserDto dto) // bool 返回值类型：表示是否修改成功，true 表示修改成功，false 表示用户不存在（需要判断的都带bool）
        {
            try
            {
                User user = new User();
                // 对象属性拷贝
                user.Id = dto.Id;
                user.Name = dto.Name;
                user.Account = dto.Account;
                user.Role = dto.Role;
                user.Status = dto.Status;
                // 手动赋值
                user.CreateTime = DateTime.Now;

                bool updateUser = m_userRepository.UpdateUser(user);    // 返回的是bool类型，并在下面加防御性编程：判断一下（true就不会进去）
                if (!updateUser)
                {
                    throw new BusinessException(BusinessErrorCode.UserNotFound, ConstantMessage.UserNotFound);//抛用户不存在的业务码和异常信息
                }

                return updateUser;   // 返回的是bool类型

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userIdClaim">JWT 中的用户 ID claim（字符串）</param>
        /// <param name="changePasswordDto"></param>
        public void ChangePassword(string? userIdClaim, ChangePasswordDto changePasswordDto)
        {
            try
            {
                // 校验id是否有效（并尝试把入参的string? userIdClaim 转换为DL层修改密码的入参int userId）
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    throw new BusinessException(BusinessErrorCode.InvalidUserIdentity, ConstantMessage.InvalidUserIdentity);
                }

                // 查询用户是否存在
                User? user = m_userRepository.GetUserById(userId);
                if (user is null)
                {
                    throw new BusinessException(BusinessErrorCode.UserNotFound, ConstantMessage.UserNotFound);
                }

                // 校验原本的密码是否正确
                string oldPassword = m_sm4.Decrypt(changePasswordDto.OldPassword);
                if (!m_sm4.Verify(oldPassword, user.Password))
                {
                    throw new BusinessException(BusinessErrorCode.InvalidCredentials, ConstantMessage.InvalidCredentials);
                }

                // 提取入参的Dto中的两次密码，并判断是否相等
                string newPassword = m_sm4.Decrypt(changePasswordDto.NewPassword);
                string confirmPassword = m_sm4.Decrypt(changePasswordDto.ConfirmPassword);
                if (newPassword != confirmPassword)
                {
                    throw new BusinessException(BusinessErrorCode.NewPasswordMismatch, ConstantMessage.NewPasswordMismatch);
                }

                // 检查新密码长度是否 > 6
                if (newPassword.Length < 6)
                {
                    throw new BusinessException(BusinessErrorCode.PasswordTooShort, ConstantMessage.PasswordTooShort);
                }

                // 加密新密码
                string NewPassword = m_sm4.Encrypt(newPassword);
                // 调用 Repository 的修改密码方法
                bool trueOrFalse = m_userRepository.UpdatePassword(userId, NewPassword);
                // 并判断是否为true（即是否修改成功）
                if (!trueOrFalse)
                {
                    throw new BusinessException(BusinessErrorCode.UserNotFound, ConstantMessage.UserNotFound);
                }

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 重置当前用户密码
        /// </summary>
        public void ResetPassword(string? userIdClaim)
        {
            try
            {
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    throw new BusinessException(BusinessErrorCode.InvalidUserIdentity, ConstantMessage.InvalidUserIdentity);
                }

                // 重置密码（并加密存储）
                string resetPassword = m_sm4.Encrypt("123456");
                if (!m_userRepository.UpdatePassword(userId, resetPassword))
                {
                    throw new BusinessException(BusinessErrorCode.UserNotFound, ConstantMessage.UserNotFound);
                }

            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }

        

        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUser(int id)
        {
            try
            {
                m_userRepository.DeleteUser(id);
                // 数据库删除成功后清除对应的用户资料缓存
                RemoveUserProfileCache(id);
            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }



        /// <summary>
        /// 启用禁用账号状态（根据用户 ID） 
        /// </summary>
        /// <param name="id"></param>
        public void EnableOrDisableAccountStatus(int id)
        {
            try
            {
                m_userRepository.EnableOrDisableAccountStatus(id);
                // 账号状态已改变，清除缓存（根据ID）
                RemoveUserProfileCache(id);
            }
            catch (Exception)
            {
                // 写日志，记录异常（并抛出异常给 Controller 统一捕获处理）
                throw;
            }
        }


    }
}
