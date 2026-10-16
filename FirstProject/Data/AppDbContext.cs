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

        // 构造函数：接收外部传入的配置（连接字符串等）
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        /// <summary>
        /// 声明实体集合：第一个 User 是实体类型，第二个 User 是 DbSet 属性名
        ///     默认情况下，EF Core 会使用“DbSet属性名”作为表名，因此映射到数据库里面 Users 表（是负数！）
        ///     因此，我在User实体类上加了[Table("user")]特性来指定表名为 user
        /// </summary>
        public DbSet<User> User { get; set; }   

    }
}
