using FirstProject.MyException;
using System.Text.Json;
using FirstProject.MyException;
using FirstProject.Result;

namespace FirstProject.Middleware
{
    /// <summary>
    /// 捕获未被 Controller 处理的异常，并转换为统一 HTTP 响应。
    /// </summary>
    public class GlobalExceptionHandler
    {

        private readonly RequestDelegate m_next;
        /// <param name="next"></param>
        public GlobalExceptionHandler(RequestDelegate next)
        {
            m_next = next;
        }




        /// <summary>
        /// 处理当前 HTTP 请求的方法（入参参数 HttpContext：包含当前请求、响应及其状态信息）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await m_next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();

                (int httpStatusCode, int responseCode, string message) = ex switch
                {
                    BusinessException businessException =>
                    (
                        BusinessErrorCodeHttpCodeMapper.Mapper(businessException.ErrorCode),
                        (int)businessException.ErrorCode,
                        businessException.Message
                    ),
                    _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        StatusCodes.Status500InternalServerError,
                        "服务器内部错误"
                    )
                };

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



















    }
}
