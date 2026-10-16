using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 注册入参Dto
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// 账号（附加“长度必须5-20位，且只能含英文”的入参校验）【注：这属于入参格式校验(长度/格式/只能保护英文...)，这些不归Service层做，Service层做业务逻辑校验(是否为空/权限够不...)】
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Za-z]{5,20}$",ErrorMessage = "账号长度必须为5到20位，且只能包含英文字母")]
        public required string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public required string Password { get; set; }
    }
}
