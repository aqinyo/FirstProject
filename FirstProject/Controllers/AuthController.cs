using FirstProject.Model.Dto;
using FirstProject.Model.VO;
using FirstProject.Result;
using FirstProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controller
{
    /// <summary>
    /// 用户认证相关接口
    /// </summary>
    [ApiController]
    [Route("api/auth")] //请求前缀
    public class AuthController:ControllerBase
    {
        private readonly IUserService m_userService;    // 依赖注入 Service层的接口
        public AuthController(IUserService userService) // 创建一个 IUserService 的实例
        {
            m_userService = userService;
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        [HttpPost("login")] // 细节：这个路由等价于"[HttpPost]+[Route("Login")]"的路由写法（这与Java有不一样的）
        public async Task<Result<LoginVo>> Login([FromBody] LoginDto dto)
        {
            var loginVo = await m_userService.Login(dto);
            return Result<LoginVo>.Success(loginVo);
        }


        /// <summary>
        /// 注册
        /// </summary>
        [HttpPost("register")]
        public async Task<Result<string>> Register([FromBody] RegisterDto dto)
        {
            await m_userService.Register(dto);
            return Result<string>.Success("注册成功！");
        }

    }
}
