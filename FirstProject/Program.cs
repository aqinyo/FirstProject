// Progeam.cs 类比 Java 的“启动类 + 全局配置中心”

using System.Text;
using FirstProject.Data;
using FirstProject.Utils;
using FirstProject.Service;
using FirstProject.Repository;
using Microsoft.EntityFrameworkCore; // 引入 EF Core 持久层框架(ORM框架)，类似于 Java 的 MyBatis
using FirstProject.Utils.Auth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;


// 创建 WebApplicationBuilder 对象，WebApplicationBuilder 是用来构建 WebApplication 对象的
var builder = WebApplication.CreateBuilder(args);


/* 服务注册中心  （类似 Java 的 @Configuration ，注册的服务可以在整个应用程序中使用，要在builder.Build()启动应用之前）*/

// 注册 Repository （的实现类，才能在 Service 里注入使用）
builder.Services.AddScoped<UserRepository>();
// 注册 Service    （的接口和实现类，才能在 Controller 里注入使用）
builder.Services.AddScoped<IUserService, UserService>();
// 注册 Controller （类似 Java 的 @RestController ，将 控制器类 注册到依赖注入容器中，开启 控制器功能）
builder.Services.AddControllers();
// 注册 DbContext  （数据库上下文）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("firstproject"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("firstproject"))
    ));
// 注册 Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "FirstProject:";// Redis 中 Key 的统一前缀 （比如：键是 user:123，，Redis 中实际的键是 FirstProject:user:123 ）
});
// API 端点服务注册 （类似 Java 的 @RequestMapping ，将 API 端点 注册到依赖注入容器中，开启 API 端点功能）
builder.Services.AddEndpointsApiExplorer();
// 注册 JwtToken
builder.Services.AddScoped<JwtToken>();
// 注册 Sm4 加密工具类
builder.Services.AddSingleton<Sm4>();
// Swagger 服务注册
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{builder.Environment.ApplicationName}.xml";                 // 生成的 XML 文件名的文档文件  （默认是项目名称 + .xml）
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);              // 生成的 XML 文件的完整路径
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);    // 让 Swagger 从该 XML 文件读取注释，并显示在 API 文档中（同时要搭配项目属性中的“生成 XML 文档文件”选项）
});



// 配置 JWT 验证参数
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("没配置 appsappsettings.Development.json 中的 Jwt:Key");
var jwtIssuer = jwt["Issuer"] ?? throw new InvalidOperationException("没配置 appsappsettings.json 中的 Jwt:Issuer");         //这里对应 appsettings.json 中的 Jwt:Issuer 配置项，如果没有配置则抛出异常并提示
var jwtAudience = jwt["Audience"] ?? throw new InvalidOperationException("没配置 appsappsettings.json 中的 Jwt:Audience");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters  // AddJwtBearer 注册并配置 JWT Bearer 身份验证处理程序 / options.TokenValidationParameters：指定验证令牌时要检查的规则（即下面的几项）
    {
        // 验证签发者Issuer (appsappsettings.json)
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        // 验证接收者Audience (appsappsettings.json)
        ValidateAudience = true,
        ValidAudience = jwtAudience,

        // 验证 Jwt:Key（appsappsettings.Development.json 开发环境 配置文件）
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        // 验证 Token 是否在有效时间
        ValidateLifetime = true
    });




 
//创建 WebApplication 对象，类似 Java 的 SpringApplication.run() 方法
var run = builder.Build();


/* 中间件管道    （管道中的中间件会按顺序执行的）*/

// 注册 全局异常处理中间件：必须放在管道最前面，它才能捕获位于它后面所有中间件/控制器类抛出的异常，没有它的话就无法统一返回异常响应体，而 Service 里 throw 的异常会变成 HTTP 500 ，前端拿不到错误提示
run.UseMiddleware<FirstProject.Middleware.GlobalExceptionHandler>();//把 GlobalExceptionHandler 加入 ASP.NET Core 的 HTTP 中间件管道，让它处理后续中间件和控制器执行时抛出的异常

// 注：这个配置的作用是在“开发环境”中提供 Swagger API 文档和交互式测试页面，没有这个控制器 API 通常仍然可以正常运行，只是 Swagger 文档和 UI 不可用（生产环境关闭）
if (run.Environment.IsDevelopment())
{
    run.UseSwagger();       // Swagger 中间件，可根据代码自动生成 API 文档，类似 Java 的 Swagger注解（Swagger UI同理）
    run.UseSwaggerUI();     // Swagger UI 中间件
}

run.UseHttpsRedirection();  // 启动HTTPS 重定向中间件，HTTPS 重定向中间件是用来将 HTTP 请求重定向到 HTTPS 的，也是加密操作
run.UseAuthentication();    // 启动身份验证中间件JWT
run.UseAuthorization();     // 启动授权中间件，类似 Java 的 @PreAuthorize 注解，授权中间件是用来验证用户是否有权限访问某个资源
run.MapControllers();       // 启动路由映射，类似 Java 的 @RequestMapping 注解

run.Run(); // 启动类的入口，类似 Java 的 main 主方法
