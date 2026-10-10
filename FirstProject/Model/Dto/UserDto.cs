using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 用户信息入参（面向admin）
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }       // 新增时必填，编辑时如果不修改密码可以留空

        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        public string Role { get; set; } = "Admin"; // 默认管理员，普通用户为User（因为是面向管理员的DTO）

        /// <summary>
        /// 账号状态
        /// </summary>
        public int Status { get; set; } = 1;        // 默认启用状态，禁用状态为0
    }
}
