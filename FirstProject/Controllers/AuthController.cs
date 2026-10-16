using FirstProject.Model.Dto;
using FirstProject.Model.Vo;
using FirstProject.Model.VO;
using FirstProject.MyException;
using FirstProject.Result;
using FirstProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FirstProject.Controller
{
    /// <summary>
    /// 用户认证相关接口
    /// </summary>
    [ApiController]
    [Route("api/[Controller]")]                         // [controller] 会根据类名 AuthController 生成 auth，类似于占位符的作用，但是会自动去掉控制器类名的Controller后缀，因此认证相关接口地址是 /api/auth/ ...（路由匹配通常不区分大小写，也可以是/api/Auth）
    public class AuthController : ControllerBase        //注：冒号左右各有一个空格
    {

        private readonly IUserService m_userService;    // 依赖注入 Service层的接口
        /// <summary>
        /// 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        /// </summary>
        /// <param name="userService"></param>
        public AuthController(IUserService userService) // 创建一个 IUserService 的实例
        {
            m_userService = userService;
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto">接收登录的DTO类</param>
        /// <returns></returns>
        [HttpPost("login")] 
        public Result<LoginVo> Login([FromBody] LoginDto loginDto)   
        {
            try
            {
                // 先new出一个空壳对象，给下面赋值进去，然后return（下面的Result<>的同理）
                LoginVo loginVo = new LoginVo();
                Result<LoginVo> loginResult = new Result<LoginVo>();

                loginVo = m_userService.Login(loginDto);
                loginResult = Result<LoginVo>.Success(loginVo);

                return loginResult;

            }
            catch (BusinessException bex)
            {
                // 日志，记异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<LoginVo>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="dto">接收注册的DTO类</param>
        /// <returns></returns>
        [HttpPost("register")]
        public Result<string>  Register([FromBody] RegisterDto dto)
        {
            try
            {
                m_userService.Register(dto);
                return Result<string>.Success("注册成功！");           

            }
            catch (BusinessException bex)
            {
                // 日志
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 获取当前登录用户资料（JWT 身份验证通过后，就从用户 ID claim 查 Redis / 数据库 ）
        /// </summary>
        /// <returns>当前用户资料</returns>
        [Authorize]
        [HttpGet("me")]
        public Result<UserVo> GetCurrentUser()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Result<UserVo>.Fail(StatusCodes.Status401Unauthorized, "登录身份无效");
            }

            UserVo? userVo = m_userService.GetUserProfile(userId);
            if (userVo is null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Result<UserVo>.Fail(StatusCodes.Status401Unauthorized, "用户不存在或账号已禁用");
            }

            return Result<UserVo>.Success(userVo);
        }



    }
}
