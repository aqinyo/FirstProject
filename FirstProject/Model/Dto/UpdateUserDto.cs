using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 修改用户信息入参（面向admin）（修改用户信息不改密码，有独立的修改密码接口）
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称（下面都有非空校验，前端不修改的字段也要把原值传出来才能不触发非空校验）
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// 账号（附加“长度必须5-20位，且只能含英文”的入参校验）【注：这属于入参格式校验(长度/格式/只能保护英文...)，这些不归Service层做，Service层做业务逻辑校验(是否为空/权限够不...)】
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Za-z]{5,20}$", ErrorMessage = "账号长度必须为5到20位，且只能包含英文字母")]
        public required string Account { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; } = "User"; // 默认用户

        /// <summary>
        /// 账号状态
        /// </summary>
        public AccountStatus Status { get; set; } = AccountStatus.Enabled;  // 默认启用状态，禁用状态为0（采用枚举，只能是0或1，避免前端传其他数字）

    }

    /// <summary>
    /// 枚举类型，表示用户状态
    /// </summary>
    public enum AccountStatus  
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 启用
        /// </summary>
        Enabled = 1
    }

}
