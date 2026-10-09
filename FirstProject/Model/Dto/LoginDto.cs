using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 登录入参
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "账号不能为空")]
        public required string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public required string Password { get; set; }
    }
}
