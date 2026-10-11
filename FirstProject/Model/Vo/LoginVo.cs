namespace FirstProject.Model.VO;

/// <summary>
/// 登录成功后返回的VO类
/// </summary>
public class LoginVo
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// JWT Token 的过期时间
    /// </summary>
    public DateTime TokenOutTime { get; set; }

    /// <summary>
    /// 登录成功后返回的用户信息，前端可以根据这个信息来显示用户的基本信息
    /// </summary>
    public UserVo? User { get; set; }

}
