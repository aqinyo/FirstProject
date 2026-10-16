using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 修改密码的入参Dto类
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 旧密码（前端传入的 SM4 密文）
        /// </summary>
        [Required]
        public required string OldPassword { get; set; }

        /// <summary>
        /// 新密码（前端传入的 SM4 密文）
        /// </summary>
        [Required]
        public required string NewPassword { get; set; }

        /// <summary>
        /// 确认密码（前端传入的 SM4 密文，由 Service 解密后比较）
        /// </summary>
        [Required]
        public required string ConfirmPassword { get; set; }

    }
}
