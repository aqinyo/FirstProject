using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 用户信息入参（面向admin，主要是修改用户的时候接收的参数）
    /// </summary>
    public class UserDto
    {
        [Required]
        public int Id { get; set; } // 编辑时必填，新增时不需要传

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }       // 新增时必填，编辑时如果不修改密码可以留空

        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        public string Role { get; set; } = "User"; // 默认用户，普通用户为User（因为是面向管理员的DTO）

        /// <summary>
        /// 账号状态
        /// </summary>
        public AccountStatus Status { get; set; } = AccountStatus.Enabled;        // 默认启用状态，禁用状态为0（采用枚举，只能是0或1，避免前端传其他数字）

    }

    // 枚举类型，表示用户状态
    public enum AccountStatus  
    {
        Disabled = 0,
        Enabled = 1
    }

}
