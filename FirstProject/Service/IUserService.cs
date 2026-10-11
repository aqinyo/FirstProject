using FirstProject.Model.Dto;
using FirstProject.Model.VO;
using FirstProject.Result;

namespace FirstProject.Service
{
    public interface IUserService
    {
        // 注册
        Task Register(RegisterDto registerDto); 
        

        // 登录
        Task<LoginVo> Login(LoginDto loginDto);


        // 根据ID查询单个用户
        Task<UserVo?> GetUserById(int id);  // Service层返回的是VO类，Repository层返回的是实体类，Controller层返回的是(统一响应体)Result类


        // 分页查询用户列表
        Task<PageResult<UserVo>> GetUserPageList(PageQueryDto pageQueryDto);    // 这里没有返回VO是因为这是分页查询，返回的是PageResult<UserVo>，而不是单个UserVo


        // 新增用户
        Task AddUser(UserDto dto);


        // 修改用户
        Task UpdateUser(UserDto dto);


        // 删除用户（根据ID）
        Task DeleteUser(int id);


        // 启用禁用账号
        Task EnableOrDisableAccountStatus(int id);
    }
}
