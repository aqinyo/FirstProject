using FirstProject.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controller
{
    /// <summary>
    /// 用户认证接口
    /// </summary>
    [ApiController]
    public class AuthController:ControllerBase
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="dto">登录入参对象</param>
        /// <returns>成功返回 Jwt Token，失败返回错误信息</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            return Ok();
        }
    }
}
