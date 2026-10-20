using FirstProject.Model.Entity; 
using FirstProject.MyException;
using FirstProject.Result;       
using FirstProject.Service;      
using System;
using System.Security.Claims;
using System.Text.Json;

namespace FirstProject.Middleware
{
    /// <summary>
    /// 捕获未被 Controller 处理的Excepiton系统级异常，并转换为统一 HTTP 响应
    /// </summary>
    public class GlobalExceptionHandler
    {
        // 依赖注入
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;


        /// <summary>
        /// 全局异常处理中间件的构造函数
        /// </summary>
        /// <param name="next">请求管道中的下一个中间件委托</param>
        /// <param name="logger">记中间件自己的运行状态，比如：写库失败时的兜底日志</param>
        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }



        /// <summary>
        /// 中间件的核心执行方法（每次 HTTP 请求经过就会自动触发）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1. 如果响应已经开始，无法修改状态码，只能原样抛出
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("响应已开始，无法处理异常中间件");
                    throw;
                }

                // 2. 定义统一返回的状态码和消息
                int httpStatusCode;
                int responseCode;
                string message;

                // 3. 判断异常类型
                if (ex is BusinessException bex)
                {
                    // 【分支 A：业务异常（自定义）】只返回错误信息给前端，不写数据库日志
                    httpStatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                    responseCode = (int)bex.ErrorCode;
                    message = bex.Message;
                }

                else
                {
                    // 【分支 B：系统异常】返回 500，并记录到 ExceptionLog 表
                    httpStatusCode = StatusCodes.Status500InternalServerError;
                    responseCode = StatusCodes.Status500InternalServerError;
                    message = "服务器内部错误，请联系管理员";

                    // 执行写入日志操作
                    WriteExceptionLog(context, ex);
                }

                // 4. 清理响应并重新设置，向前端输出 JSON
                context.Response.Clear();
                context.Response.StatusCode = httpStatusCode;
                context.Response.ContentType = "application/json; charset=utf-8";

                var result = Result<object>.Fail(responseCode, message);
                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        }




        /// <summary>
        /// 组装并同步写入异常日志（写日志的方法就落到全局异常处理器这里了）
        /// </summary>
        private void WriteExceptionLog(HttpContext context, Exception ex)
        {
            try
            {
                // 必须通过 RequestServices 获取 Scoped 服务
                var logService = context.RequestServices.GetService<IExceptionLogService>();
                if (logService == null) return;

                // 尝试从 User 身份信息中提取操作人
                int? operatorId = null;
                string operatorName = "未知";
                var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
                {
                    operatorId = parsedId;
                    operatorName = context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "未知";
                }

                // 组装实体数据
                var logEntity = new ExceptionLog
                {
                    BusinessModule = "System", // 可改为动态提取
                    OperatorId = operatorId,
                    OperatorName = operatorName,
                    RequestUrl = context.Request.Path,
                    RequestMethod = context.Request.Method,

                    // 全局中间件读取入参容易报错，留空；需要的话建议在 Filter 中存进 HttpContext.Items 再取
                    RequestParam = "",

                    ExceptionMsg = ex.Message,
                    ExceptionStack = ex.ToString(), // 包含完整内部异常堆栈
                    CreateTime = DateTime.Now
                };

                // 【关键修改点】：调用你的接口方法 InsertExceptionLog()
                logService.InsertExceptionLog(logEntity);
            }

            catch (Exception logEx)
            {
                // 兜底：防止写日志本身报错导致程序崩溃
                _logger.LogError(logEx, "写入 ExceptionLog 表失败！");
            }

        }
    }
}