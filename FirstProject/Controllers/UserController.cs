using FirstProject.Model.Dto;
using FirstProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controller
{
    /// <summary>
    /// 用户管理接口
    /// </summary>
    [ApiController]
    [Route("api/[users]")] //请求前缀
    public class UserController : ControllerBase
    {
        private readonly IUserService m_userService; // 依赖Service层
        public UserController(IUserService userService)
        {
            m_userService = userService;
        }


        [HttpGet]
        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        public async Task<IActionResult> GetUserList(   // Task是 C# 异步编程（async/await）的核心，Task<T> 代表异步执行完毕后会返回 T 类型结果，Task 代表异步，无返回值
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? keyword = null)
        {
            var list = await m_userService.GetUserList(page, size, keyword); // var = 隐式类型推断，让编译器自动识别变量类型，不用手写。等价于List<T>
            var total = await m_userService.GetTotalCount(keyword);
            return Ok(new { code = 200, data = new { list, total } });  // IActionResult：控制器方法返回类型，是接口，Ok() 是它的实现，代表 HTTP 200 成功响应
        }   // new { code = 200, data = new { list, total } }：匿名类。临时创建一个没有类名的对象，快速组装返回 JSON

        [HttpGet("{id}")]
        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        public async Task<IActionResult> GetUserById（int Id）{}












    }
}
