using FirstProject.Model.Dto;
using FirstProject.Model.VO;
using FirstProject.MyException;
using FirstProject.Result;
using FirstProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;  //在 ASP.NET Core 中，Web API 控制器（[ApiController]特性）本质上就是 MVC 控制器的一种特化形式。而从 ASP.NET Core 1.0 开始，微软就将 MVC 和 Web API 合并成了统一的编程模型了
using System.Security.Claims;
                                 //这段代码本质是ASP.NET Core MVC 框架开发的 Web API ，因为 Web API 是通过 HTTP 接口提供数据的开发方式，而你代码里面使用的 [ApiController]、[HttpGet]、[FromBody] 等也都体现了 API 控制器的用法

namespace FirstProject.Controller
{
    /// <summary>
    /// 用户管理相关接口
    /// </summary>
    [Authorize(Roles = "Admin")]                            // 只有Admin角色的用户才能访问该接口（而AuthController没加是公共接口，登录注册谁都可以用）
    [ApiController]
    [Route("api/[Controller]")]                             // 接口地址 /api/user
    public class UserController : ControllerBase            //注：冒号左右各有一个空格
    {

        /// <summary>
        /// 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        /// </summary>
        private readonly IUserService m_userService;        // 依赖注入 Service层的接口
        public UserController(IUserService userService)     // 创建一个 IUserService 的实例
        {
            m_userService = userService;
        }



        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        /// <param name="id">传用户id参数即可（name必须和入参名一致）</param>
        /// <returns>返回查询到的用户VO类的数据；找不到时返回 null（return标签这里可以为空）</returns>
        [HttpGet("{id}")]
        public Result<UserVo> GetUserById(int id)                // [ApiController]下简单类型(int/string/DateTime)默认就 FromQuery，可以省略不写[FromQuery]
        {
            try
            {
                UserVo userVo = m_userService.GetUserById(id);  // 因为返回值是UserVo，所以这里建议用强类型转换，不要用弱类型var（注：类名是驼峰命名，局部变量名是首个字母小写后面再驼峰命名）
                return Result<UserVo>.Success(userVo);

            }
            catch (BusinessException bex)                       // 捕获的异常是我自定义的异常类/预期的异常都在这BusinessException内，然后一般的/不可预期/未在BusinessException里面的异常由全局异常处理器统一捕获兜底处理
            {
                //写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<UserVo>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        /// <param name="pageQueryDto">分页查询入参的DTO类参数（分页查询推荐用[FromQuery]查询参数，会自动从URL中提取。其余的Post请求则是[FromBody]多）</param>
        /// <returns></returns>
        [HttpGet("GetUserPageList")]
        public Result<PageResult<UserVo>> GetUserPageList([FromQuery] PageQueryDto pageQueryDto)
        {
            try
            {
                PageResult<UserVo> pageResult = new PageResult<UserVo>();
                Result<PageResult<UserVo>> pageQueryResult = new Result<PageResult<UserVo>>();

                pageResult = m_userService.GetUserPageList(pageQueryDto);
                pageQueryResult = Result<PageResult<UserVo>>.Success(pageResult);

                return pageQueryResult;

            }
            catch (BusinessException bex)
            {
                //写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<PageResult<UserVo>>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 动态多条件查询用户
        /// </summary>
        /// <param name="queryDto">账号/名称/状态</param>
        /// <returns></returns>
        [HttpGet("GetUserPageQueryList")]
        public Result<PageResult<UserVo>> GetUserPageQueryList([FromQuery] UserQueryDto queryDto)
        {
            try
            {
                // 先new出一个空壳对象，给下面赋值进去，然后return（下面的Result<>的同理）
                PageResult<UserVo> pageQuery = new PageResult<UserVo>();
                Result<PageResult<UserVo>> pageQueryResult = new Result<PageResult<UserVo>>();

                pageQuery = m_userService.GetUserPageQueryList(queryDto);
                pageQueryResult = Result<PageResult<UserVo>>.Success(pageQuery);    // 然后把service层返回的数据(data)放到Result的data当中

                return pageQueryResult;                                             // 业务正常就直接返回，失败就走到catch中返回异常结果集

            }
            catch (BusinessException bex) // 捕获的异常是我自定义的异常类。预期的异常都在这BusinessException内，然后一般的/不可预期/未在BusinessException里面的异常由全局异常处理器统一捕获兜底处理
            {
                //写日志，记录异常(不能和service层那样子抛出异常了)

                // 调用我的Mapper把“自定义的业务码”映射为“标准的HTTP状态码”，给下面return给前端
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);

                // 同时这里调用Result结果集的(含code和msg)失败方法，返回我自定义异常类里面的业务码和msg异常信息
                return Result<PageResult<UserVo>>.Fail((int)bex.ErrorCode, bex.Message); 
            }
        }



        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="addUserDto">用户信息的接受参数，是AddUserDto类</param>
        /// <returns></returns>
        [HttpPost("add")]
        public Result<string> AddUser([FromBody] AddUserDto addUserDto)
        {
            try
            {
                m_userService.AddUser(addUserDto);
                return Result<string>.Success("新增成功！");     

            }
            catch (BusinessException bex)
            {
                //写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="dto">用户信息的接受参数，是UserDto类</param>
        /// <returns></returns>
        [HttpPost("update")]
        public Result<string> UpdateUser([FromBody] UpdateUserDto dto)
        {
            try
            {
                m_userService.UpdateUser(dto);
                return Result<string>.Success("修改成功！");

            }
            catch (BusinessException bex)
            {
                //写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changePasswordDto"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        public Result<string> Password(ChangePasswordDto changePasswordDto)
        {
            try
            {
                // 从当前请求的已认证用户信息中读取用户 ID（ClaimTypes.NameIdentifier 表示“用户标识”这一类声明） 
                string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
                m_userService.ChangePassword(userIdClaim, changePasswordDto);
                return Result<string>.Success("修改密码成功！");

            }
            catch (BusinessException bex)
            {
                // 写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 重置密码（固定重置为123456，搭配“修改密码”使用）
        /// </summary>
        [HttpPost("ResetPassword")]
        public Result<string> ResetMyPassword()
        {
            try
            {
                string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                m_userService.ResetPassword(userIdClaim);
                return Result<string>.Success("密码已重置！");
            }
            catch (BusinessException bex)
            {
                // 写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        /// <param name="id">传用户id参数即可, [ApiController]下简单类型(int/string/DateTime)默认就 FromQuery，可以省略不写[FromQuery]</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public Result<string> DeleteUser(int id)
        {
            try
            {
                m_userService.DeleteUser(id);
                return Result<string>.Success("删除成功!");

            }
            catch (BusinessException bex)
            {
                // 写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }



        /// <summary>
        /// 启用禁用账号状态
        /// </summary>
        /// <param name="id">传用户id参数即可</param>
        /// <returns></returns>
        [HttpPost("EnableOrDisableAccountStatus")]
        public Result<string> EnableOrDisableAccountStatus(int id)
        {
            try
            {
                m_userService.EnableOrDisableAccountStatus(id);
                return Result<string>.Success("操作成功!");

            }
            catch (BusinessException bex)
            {
                // 写日志，记录异常
                Response.StatusCode = BusinessErrorCodeHttpCodeMapper.Mapper(bex.ErrorCode);
                return Result<string>.Fail((int)bex.ErrorCode, bex.Message);
            }
        }

    }
}
