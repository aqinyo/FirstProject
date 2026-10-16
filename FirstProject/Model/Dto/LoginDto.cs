using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 登录入参（注：我的Model模型 → Dto、实体、Vo → 都是只有属性/字段存在，赋值操作全在Service层完成，Dto还会有个入参的非空校验）
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 账号 
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Za-z]{5,20}$", ErrorMessage = "账号长度必须为5到20位，且只能包含英文字母")]
        public required string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public required string Password { get; set; }
    }
}
