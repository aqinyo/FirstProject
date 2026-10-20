/*
  DbContext.cs 的主要职责：声明有什么表、管理数据库连接和事务、配置映射规则（可选）

    注：具体的数据库连接配置还需要在 appsettings.json 中进行配置（实际上，Java的application.yml  约等于  C# 的 AppDbContext + appsettings.json）
    DbContext：	是基础设施。它知道怎么连数据库、怎么把 C# 对象翻译成 SQL、怎么执行事务。它暴露 DbSet<T> 供上层 Service层 使用  
    而我自定义的 xxxDbContext，就负责继承这个 DbContext 获得其功能，同时还得去 Program.cs 里注册它
*/

using FirstProject.Model.Entity;
using Microsoft.EntityFrameworkCore; 

namespace FirstProject.Data
{
    public class AppDbContext : DbContext      // 继承自 EF Core 的 DbContext 类，EF Core 的核心类就是 DbContext，它是数据库的“化身”或“超级代理”，它知道怎么连数据库、怎么把 C# 对象翻译成 SQL、怎么执行事务
    {

        /// <summary>
        /// 构造函数：接收外部传入的配置（连接字符串等）
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        /// <summary>
        /// 表声明/声明实体集合：第一个 User 是实体类型，第二个 User 是 DbSet 属性名
        ///         默认情况下，EF Core 会使用“DbSet属性名”作为表名，因此映射到数据库里面 Users 表（是负数！）
        ///         因此，我在User实体类上加了[Table("user")]特性来指定表名为 user
        /// </summary>
        public DbSet<User> User { get; set; }   

        public DbSet<StatusLog> StatusLog { get; set; }

        public DbSet<ExceptionLog> ExceptionLog { get; set; }






        /// <summary>
        /// 全局过滤器 配置 （换成“软删”后加入的，作用是：每次查数据库时，不需要手动加“是否被删”的判断SQL，而是全局过滤器自动帮你的 SQL 加上 WHERE DeleteTime = '2026-10-01'去判断）
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 设置默认的未删除时间常量（方便复用）
            var defaultTime = new DateTime(2026, 10, 1);

            // 配置全局查询过滤器 (以后查这张表自动带上这个条件)
            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeleteTime == defaultTime);

            base.OnModelCreating(modelBuilder);
        }



        /// <summary>
        /// 重写SaveChanges（原本是“提交变更操作给数据库”的，现在重写，加个“拦截物理删除，将其修改为更新形式的软删”功能，然后执行查询/删除等照常写原本的SQL即可）
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            // 拦截所有准备执行 Delete 操作的数据
            var deletedEntries = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in deletedEntries)
            {
                // 将状态从 Delete 改为 Update
                entry.State = EntityState.Modified;
                // 赋值当前时间（这里就是删除的时间了）
                entry.Entity.DeleteTime = DateTime.Now;
            }

            // 这里返回的是同步的逻辑
            return base.SaveChanges(); 
        }

        /// <summary>
        /// 重写SaveChangesAsync，和重写SaveChanges一模一样，只是这里针对的是异步的“提交变更操作给数据库”
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedEntries = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in deletedEntries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeleteTime = DateTime.Now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
