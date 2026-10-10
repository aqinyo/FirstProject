using FirstProject.Model.Dto;
using FirstProject.Model.VO;
using FirstProject.Result;
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

        private readonly IUserService m_userService;    // 依赖注入 Service层的接口
        public UserController(IUserService userService) // 创建一个 IUserService 的实例
        {
            m_userService = userService;
        }
         

        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        [HttpGet("{id}")]   // 这里的{id}是路由参数，表示URL中会有一个动态的ID值，例如/api/users/1
        public async Task<Result<UserVo>> GetUserById([FromRoute] int id)   // Task是 C# 异步编程（async/await）的核心，Task<T> 代表异步执行完毕后会返回 T 类型结果，Task 代表异步，无返回值
        {
            var userVo = await m_userService.GetUserById(id);               // 注：类名是驼峰命名，局部变量名是首个字母小写后面再驼峰命名
            return Result<UserVo>.Success(userVo);                          // 可能为空的情况，Controller层不处理，直接返回Result.Success(userVo)，Service层处理
        }


        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        [HttpGet]
        public async Task<Result<PageResult<UserVo>>> GetUserPageList([FromQuery] PageQueryDto pageQueryDto)
        {
            var pageResult = await m_userService.GetUserPageList(pageQueryDto);
            return Result<PageResult<UserVo>>.Success(pageResult);
        }


        /// <summary>
        /// 新增用户
        /// </summary>
        [HttpPost]
        [Route("AddUser")]
        public async Task<Result<string>> AddUser([FromBody] UserDto dto)   //async修饰符表示示这个方法内部会使用 await 等待异步操作
        {
            await m_userService.AddUser(dto);                // 用到await，是指在调用Service层的 AddUser 方法时，程序会等待该方法执行完成后再继续执行后续代码
            return Result<string>.Success("新增成功！");     // 这里不返回数据，前端不需要返回VO数据，只需要修改知道成功与否（所以是String类型）以下同理
        }
         
        
        /// <summary>
        /// 修改用户（先查后改）
        /// </summary>
        [HttpPut]
        public async Task<Result<string>> UpdateUser([FromBody] UserDto dto)
        {
            await m_userService.UpdateUser(dto);
            return Result<string>.Success("修改成功！");    
        }


        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        [HttpDelete]
        public async Task<Result<string>> DeleteUser([FromQuery] int id)
        {
            await m_userService.DeleteUser(id);
            return Result<string>.Success("删除成功!");//注意还有用户不存在的情况，但还是Service层处理，Controller层不处理
        }


        /// <summary>
        /// 启用禁用账号状态
        /// </summary>
        [HttpPost]
        [Route("enableOrDisableAccountStatus")] //加上路由匹配，是因为避免和“新增用户”的路由冲突,Post请求有两个，所以我都加了路由来区分（其他的只有一个，所以不需要加路由）
        public async Task<Result<string>> EnableOrDisableAccountStatus([FromQuery] int id)
        {
            await m_userService.EnableOrDisableAccountStatus(id);
            return Result<string>.Success("操作成功!");
        }

    }
}
