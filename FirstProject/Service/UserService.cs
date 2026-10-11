using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Model.VO;
using FirstProject.Repository;
using FirstProject.Result;
using FirstProject.Utils.Auth;  
using FirstProject.Utils;
using Mapster;                      // 引入Mapster命名空间，作用是将DTO类和实体类之间进行映射，类似于Java中的BeanUtils.copyProperties()对象属性拷贝方法
namespace FirstProject.Service
{
    /// <summary>
    /// 用户管理实现类
    /// </summary>
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;   // 依赖注入 Repository层 的接口
        private readonly JwtToken _jwtToken;                // 依赖注入自定义的 JwtToken 工具类，用于生成JWT令牌
        private readonly Sm4 _sm4;                          // 依赖注入自定义的 Sm4 工具类，用于密码加密和解密
        public UserService(IUserRepository userRepository, JwtToken jwtToken, Sm4 sm4)  // 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        {
            _userRepository = userRepository;
            _jwtToken = jwtToken;
            _sm4 = sm4;
        }


        /// <summary>
        /// 登录
        /// </summary>
        public async Task<LoginVo> Login(LoginDto loginDto)
        {
                                                // 直接从 DTO中 取出Repository层 Login 方法要的入参参数“loginDto.Account”即可
            var user = await _userRepository.Login(loginDto.Account);  // 调用 Repository 层的 Login 方法，根据账号查询用户信息，返回的数据类型是 User 实体类对象，如果没有找到则返回 null

            if (user is null)
            {
                throw new Exception("账号或密码错误！");
            }

            if (user.Status != AccountStatus.Enabled)
            {
                throw new Exception("该账号已被禁用！");    
            }

            if (!_sm4.Verify(loginDto.Password, user.Password)) //调用自定义的 Sm4 工具类的 Verify 方法，验证用户输入的密码是否与数据库中存储的加密密码匹配
            {
                throw new Exception("账号或密码错误！");
            }

            var TokenOutTime = DateTime.UtcNow.AddHours(1);     // 设置JWT令牌的过期时间为1小时后

            var loginVo = new LoginVo
            {
                AccessToken = _jwtToken.CreateToken(user),     // 调用自定义的 JwtToken 工具类的 CreateToken 方法生成 JWT Token，赋值给 LoginVo 的 AccessToken 属性
                TokenOutTime = TokenOutTime,                   // 设置JWT令牌的过期时间
                User = user.Adapt<UserVo>()                    // 将 User 实体对象映射为 UserVo 对象返回（因为密码要屏蔽掉）
            };
            return loginVo;
        }


        /// <summary>
        /// 注册
        /// </summary>
        public async Task Register(RegisterDto registerDto)   //是Task<T>就要有return，Task就不需要return  （这里没加是因为注册后只需要告诉注册成功即可，不需要返回其他，要返回也是controller层返回，service没有））
        {
            // 先判断账号是否已经存在
            var isExist = await _userRepository.CheckAccountExists(registerDto.Account);
            if (isExist)
            {
                throw new Exception("该账号已被注册，请更换账号！");
            }

            // 对象属性拷贝
            var userRegister = registerDto.Adapt<User>();
            // 手动赋值
            userRegister.Password = _sm4.Encrypt(registerDto.Password); // 调用自定义的 Sm4 工具类的 Encrypt(加密)方法对用户输入的密码进行加密，并赋值给 User 实体对象的 Password 属性
            userRegister.Role = "User";                                 // 默认注册的用户角色为普通用户
            userRegister.Status = AccountStatus.Enabled;                // 默认注册的用户状态为启用
            userRegister.CreateTime = DateTime.Now;

            await _userRepository.Register(userRegister);
        }


        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        public async Task<UserVo?> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id); 
            return user?.Adapt<UserVo>();  // 将查询到的 User 实体对象映射为 UserVo 对象返回 (因为密码要屏蔽掉)   
        }


        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        public async Task<PageResult<UserVo>> GetUserPageList(PageQueryDto pageQueryDto)
        {
            // 1. 调用 Repository 获取 Entity 分页数据
            var userPage = await _userRepository.GetUserPageList(pageQueryDto); // 数据层返回的是 PageResult<User> 类型

            // 2. 将 Entity 映射为 Vo 类型
            var voList = userPage.Data?.Adapt<List<UserVo>>();

            // 3. 返回新的 PageResult<UserVo> 给Controller层
            return new PageResult<UserVo>
            {
                Total = userPage.Total,
                Page = userPage.Page,
                PageSize = userPage.PageSize,
                Data = voList
            };
        }


        /// <summary>
        ///  新增用户
        /// </summary>
        public async Task AddUser(UserDto dto)
        {
            //对象属性拷贝，将UserDto对象转换为User实体对象，实体类有但 dto 里没有的字段没有的属性下面再手动赋值
            var user = dto.Adapt<User>();
            // 手动赋值
            if (!string.IsNullOrEmpty(dto.Password))// 密码赋值前先判断是否Null
            {
                user.Password = _sm4.Encrypt(dto.Password);//加密赋值
            }
            user.CreateTime = DateTime.Now;

            await _userRepository.AddUser(user);
        }


        /// <summary>
        /// 修改用户（先查后改）
        /// </summary>
        public async Task UpdateUser(UserDto dto)
        {
            // 对象属性拷贝
            var user = dto.Adapt<User>();
            //手动赋值
            user.CreateTime = DateTime.Now;
            await _userRepository.UpdateUser(user);
        }


        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
        }


        /// <summary>
        /// 启用禁用账号状态
        /// </summary>
        public async Task EnableOrDisableAccountStatus(int id)
        {
            await _userRepository.EnableOrDisableAccountStatus(id);
        }

    }
}
