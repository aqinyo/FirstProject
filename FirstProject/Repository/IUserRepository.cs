/*
 Repository层接口是提供给Service层调用的，Service层接口是提供给Controller层调用的，Controller层接口是提供给前端调用的
*/

using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Result;

namespace FirstProject.Repository
{ /* 注：Repository层 的接口方法和 Service层 的接口方法基本一致
        只是 Repository层 方法返回的参数类型基本是 实体类，
        而 Service层 的方法返回的参数类型是 VO 类，
        （ Controller层 的方法返回的参数类型，则是统一封装的响应体 Result 类，携带着Service层返回的VO类数据 ）
  */
    public interface IUserRepository
    {
        // 注册
        Task Register(User user);


        // 登录（根据账号查询用户信息）
        Task<User?> Login(string account);


        // 查重（根据账号）
        Task<bool> CheckAccountExists(string account);


        // 根据ID查询单个用户
        Task<User?> GetUserById(int id);

        
        // 分页查询用户列表
        Task<PageResult<User>> GetUserPageList(PageQueryDto pageQueryDto);  // Repository层返回的是实体类User，而Service层返回的是VO类UserVO


        // 新增用户
        Task AddUser(User user);


        // 修改用户
        Task<bool> UpdateUser(User user);          // Service层已经对象属性映射成User实体类了，所以这里直接传入User实体类即可    


        // 删除用户（根据ID）
        Task DeleteUser(int id);                        
       

        // 启用禁用账号
        Task EnableOrDisableAccountStatus(int id);   
    }
}
