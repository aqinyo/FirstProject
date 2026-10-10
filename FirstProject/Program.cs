/****************************************************
 Progeam.cs 类比 Java 的“启动类 + 全局配置中心”
 ****************************************************/

using FirstProject.Data;
using FirstProject.Repository;
using FirstProject.Service;
using FirstProject.Utils.Auth;
using Microsoft.EntityFrameworkCore;           // 引入 Entity Framework Core 的命名空间，作用是将 C# 对象映射到数据库表，并提供 LINQ 查询功能，简化数据库操作，类似于 Java 的 MyBatis


// 创建 WebApplicationBuilder 对象，WebApplicationBuilder 是用来构建 WebApplication 对象的
var builder = WebApplication.CreateBuilder(args);



// 服务注册中心   （类似 Java 的 @Configuration ，注册的服务可以在整个应用程序中使用，要在builder.Build()之前）
builder.Services.AddControllers();              // 控制器服务注册      （类似 Java 的 @RestController ，将 控制器类 注册到依赖注入容器中，开启 控制器功能）
builder.Services.AddEndpointsApiExplorer();     // API 端点服务注册    （类似 Java 的 @RequestMapping ，将 API 端点 注册到依赖注入容器中，开启 API 端点功能）
builder.Services.AddSwaggerGen();               // Swagger 服务注册    （类似 Java 的 @EnableSwagger2 ，将 Swagger 注册到依赖注入容器中，开启 Swagger 功能）
// 注册 DbContext     （数据库上下文）
builder.Services.AddDbContext<AppDbContext>(options =>          
    options.UseMySql(
        builder.Configuration.GetConnectionString("FirstProjectDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("FirstProjectDB"))
    ));
// 注册 Repository     （的接口和实现类，才能在 Service 里注入使用，Service同理（类似 Java 的 @Autowired 注解）
builder.Services.AddScoped<IUserRepository, UserRepository>();  // 也得
// 注册 Service       （的接口和实现类，才能在 Controller 里注入使用）
builder.Services.AddScoped<IUserService, UserService>();
// 注册 JwtToken
builder.Services.AddScoped<JwtToken>();



//创建 WebApplication 对象，类似 Java 的 SpringApplication.run() 方法
var app = builder.Build();


// 中间件管道    （管道中的中间件是按顺序执行的）
// 注：这个配置的作用是在“开发环境”中提供 Swagger API 文档和交互式测试页面，没有这个控制器 API 通常仍然可以正常运行，只是 Swagger 文档和 UI 不可用（生产环境关闭）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Swagger 中间件，可根据代码自动生成 API 文档，类似 Java 的 Swagger注解（Swagger UI同理）
    app.UseSwaggerUI();     // Swagger UI 中间件
}


// 配置 CORS 跨域配置（联调用）



app.UseHttpsRedirection();  // HTTPS 重定向中间件，HTTPS 重定向中间件是用来将 HTTP 请求重定向到 HTTPS 的，也是加密操作
app.UseAuthorization();     // 授权中间件，类似 Java 的 @PreAuthorize 注解，授权中间件是用来验证用户是否有权限访问某个资源
app.MapControllers();       // 路由映射，类似 Java 的 @RequestMapping 注解，路由映射是将请求路径映射到对应的控制器方法上



app.Run(); // 启动类的入口，类似 Java 的 main 方法
