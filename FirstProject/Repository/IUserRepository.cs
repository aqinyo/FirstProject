using FirstProject.Model.Entity;

namespace FirstProject.Repository
{
    public interface IUserRepository
    {
        // 根据ID查询单个用户
        Task<User?> GetUserById(int id);


        // 分页查询用户列表
        Task<List<User>> GetUserList(int page, int size, string? keyword);


        // 总条数
        Task<int> GetTotalCount(string? keyword);


        // 新增用户
        Task AddUser(User user);


        // 修改用户信息
        Task UpdateUser(User user);              
        

        // 删除用户（根据ID）
        Task DeleteUser(User user);                        
       

        // 查看账号状态
        Task<bool> GetAccountStatus(string account);   
    }
}
