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
        Task<string> Login(LoginDto loginDto);


        // 根据ID查询单个用户
        Task<UserVo?> GetUserById(int id); 


        // 分页查询用户列表
        Task<PageResult<UserVo>> GetUserPageList(PageQueryDto pageQueryDto); 

        
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
