/*******************************************
 C#开发的Repository层与Java的 MyBatis 不太一样，Java 的 MyBatis 是写 SQL 语句的，而 C# 开发的 Repository 层是写 C# 对象操作的（LINQ 表达式），EF Core 会帮你把 C# 对象操作翻译成 SQL 语句去执行
 *******************************************/

using FirstProject.Data;
using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Result;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Repository
{
    public class UserRepository : IUserRepository
    {
           
        private readonly AppDbContext _context;     // 依赖注入的数据库上下文对象，用于操作数据库的 （readonly ：只能在构造函数里面赋值一次）
        public UserRepository(AppDbContext context) // 构造函数注入 AppDbContext
        {
            _context = context;
        }
        /*
         DbContext提供的常用方法：（所以Repository层的实现类用EFcore提供的方法去实现Repository层的接口方法，然后Service层的实现类去依赖注入Repository层的接口）
            .FindAsync(id)：             按主键查
            .FirstOrDefaultAsync(条件)： 查第一条
            .Where(条件)：               过滤
            .AddAsync(entity)：          新增
            .Remove(entity)：            删除
            .ToListAsync()：             转成列表
         */



        /// <summary>
        /// 注册
        /// </summary>
        public async Task Register(User user)
        {
            // 将实体添加到 EF Core 的变更追踪器中（此时还未真正写入数据库）
            await _context.Users.AddAsync(user);

            // 提交更改，EF Core 此时会生成 INSERT SQL 语句并执行
            await _context.SaveChangesAsync();
        }



        /// <summary>
        /// 查重（根据账号）
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountExists(string account)
        {
            return await _context.Users.AnyAsync(u => u.Account == account);    // 与"根据ID查询单个用户"的方法不同，这里是查重，所以用 AnyAsync() 方法来进行判断，它会返回一个布尔值，表示是否存在满足条件的记录
        }


        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        public async Task<User?> GetUserById(int id)    // 写Repository 层直接上 async + await 即可(Task/Task<T>看是否需要返回值，一般查询是要返回值，增删改不用)
        {
            return await _context.Users.FindAsync(id);  // FindAsync 是 EF Core 提供的根据主键查询单个实体的方法，返回值是 User类型，带? / 打个? 表示可能返回 null
        }


        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        public async Task<PageResult<User>> GetUserPageList(PageQueryDto pageQueryDto)
        {
            var query = _context.Users.AsQueryable();   // AsQueryable() 是 EF Core 提供的把 DbSet<User> 转成 IQueryable<User> 的方法，方便后续拼接查询条件

            int total = await query.CountAsync();
            int page = pageQueryDto.Page > 0 ? pageQueryDto.Page : 1;
            int pageSize = pageQueryDto.PageSize > 0 ? pageQueryDto.PageSize : 10;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<User>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Data = data
            };
        }


        /// <summary>
        /// 新增用户
        /// </summary>
        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();      // 提交事务
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        public async Task<bool> UpdateUser(User user)   // bool 返回值表示是否修改成功，true 表示修改成功，false 表示用户不存在（需要判断的都带bool）
        {
            var existingUser = await _context.Users.FindAsync(user.Id); // 先查用户信息出来

            existingUser.Name = user.Name;                          // 修改用户默认是修改基础信息，账号、密码、创建时间不修改（因此没有手动赋值密码）
            existingUser.Role = user.Role;
            existingUser.Status = user.Status;

            var rowsAffected = await _context.SaveChangesAsync();   // 提交事务，返回受影响的行数
            return rowsAffected > 0;                                // 如果受影响的行数大于 0，说明修改成功，返回 true，否则返回 false
        }


        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        public async Task DeleteUser(int id)
        {
            var deleteUser = await _context.Users.FindAsync(id);
            _context.Users.Remove(deleteUser);               
            await _context.SaveChangesAsync();          // 提交事务
        }


        /// <summary>
        /// 启用禁用账号
        /// </summary>
        public async Task EnableOrDisableAccountStatus(int id)
        { 
            var manageAccount = await _context.Users.FindAsync(id);      // 根据ID查询单个用户，所以用 FindAsync() 方法即可
            manageAccount.Status = manageAccount.Status == 1 ? 0 : 1;    // 1 为启用，0 为禁用
            await _context.SaveChangesAsync();                           // 提交事务
        }


    }
}
