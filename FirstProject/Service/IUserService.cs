using FirstProject.Model.Dto;
using FirstProject.Model.VO;

namespace FirstProject.Service
{
    public interface IUserService
    {
        // 注册
        Task<UserVo> Register(RegisterDto dto); 
        

        // 登录
        Task<string> Login(LoginDto dto);


        // 根据ID查询单个用户
        Task<UserVo?> GetUserById(int id);


        // 分页查询用户列表
        Task<List<UserVo>> GetUserList(int page, int size, string? keyword);


         // 总条数
        Task<int> GetTotalCount(string? keyword);


        // 新增用户
        Task<UserVo> AddUser(UserDto dto);


        // 修改用户(先查后改)
        Task<UserVo> UpdateUser(UserDto dto);


        // 删除用户（根据ID）
        Task DeleteUser(int id);
    }
}
