using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Model.VO;
using FirstProject.Repository;
using FirstProject.Result;
using Mapster;                      // 引入Mapster命名空间，作用是将DTO类和实体类之间进行映射，类似于Java中的BeanUtils.copyProperties()对象属性拷贝方法
namespace FirstProject.Service
{
    /// <summary>
    /// 用户管理实现类
    /// </summary>
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;   // 依赖注入 Repository层 的接口
        public UserService(IUserRepository userRepository)  // 构造函数注入 Repository层 的接口
        {
            _userRepository = userRepository;
        }


        /// <summary>
        /// 登录
        /// </summary>
        public Task<string> Login(LoginDto loginDto)
        {
            
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
            // SM4加密密码操作（非手动赋值部分）【写好SMD的类和方法后再改】
            //userRegister.Password = HashPassword(dto.Password);
            // 手动赋值
            userRegister.Role = "User";                 // 默认注册的用户角色为普通用户
            userRegister.Status = 1;                    // 默认注册的用户状态为启用
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

            // 2. 将 Entity 映射为 Vo 类型（使用 LINQ 的 Select 手动映射，mapper的有点麻烦）【密码字段不+】
            var voList = userPage.Data?.Select(u => new UserVo
            {
                Id = u.Id,
                Name = u.Name,
                Account = u.Account,
                Role = u.Role,
                Status = u.Status,
                CreateTime = u.CreateTime
            }).ToList();

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
        ///  新增用户（管理员新增）
        /// </summary>
        public async Task AddUser(UserDto dto)
        {
            //对象属性拷贝，将UserDto对象转换为User实体对象，实体类有但 dto 里没有的字段没有的属性下面再手动赋值
            var user = dto.Adapt<User>();
            // SM4加密密码操作（非手动赋值部分）【写好SMD的类和方法后再改】
            user.Password = HashPassword(dto.Password); 
            // 手动赋值
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
