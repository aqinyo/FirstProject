using FirstProject.Model.Dto;
using FirstProject.Model.Vo;
using FirstProject.Model.VO;
using FirstProject.Result;

namespace FirstProject.Service
{
    /// <summary>
    /// 用户管理业务接口
    /// </summary>
    public interface IUserService
    {

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        void Register(RegisterDto registerDto);


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        LoginVo Login(LoginDto loginDto);


        /// <summary>
        /// 根据用户 ID 获取用户资料（优先读取缓存）
        /// </summary>
        /// <param name="userId">当前登录用户 ID</param>
        /// <returns>用户资料；用户不存在或账号禁用时返回 null</returns>
        UserVo? GetUserProfile(int userId);


        /// <summary>
        /// 根据ID查询单个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserVo? GetUserById(int id);                                      // Service层返回的是VO类，Repository层返回的是实体类，Controller层返回的是(统一响应体)Result类


        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        /// <param name="pageQueryDto"></param>
        /// <returns></returns>
        PageResult<UserVo> GetUserPageList(PageQueryDto pageQueryDto);    // 这里没有返回VO是因为这是分页查询，返回的是PageResult<UserVo>，而不是单个UserVo


        /// <summary>
        /// 动态多条件查询用户（单个/多个）（按ID/名称/账号/状态 查询）
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns></returns>
        PageResult<UserVo> GetUserPageQueryList(UserQueryDto queryDto);


        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="dto"></param>
        void AddUser(AddUserDto dto);                                  // 没有返回值所以使用void，和java一样


        // 修改用户
        bool UpdateUser(UpdateUserDto dto);                               // TODO：这里的返回值类型暂定bool，判断的，但是后续要细改的


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changePasswordDto"></param>
        void ChangePassword(string? userIdClaim, ChangePasswordDto changePasswordDto);


        /// <summary>
        /// 重置当前用户密码
        /// </summary>
        void ResetPassword(string? userIdClaim);


        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        /// <param name="id"></param>
        void DeleteUser(int id);


        /// <summary>
        /// 启用禁用账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operatorIdClaim">当前操作人的用户ID</param>
        /// <param name="operatorName">当前操作人的账号</param>
        /// <param name="reason">禁用原因</param>
        void EnableOrDisableAccountStatus(int id, string? operatorIdClaim, string? operatorName, string? reason);


    }
}
