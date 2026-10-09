using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 用户信息入参
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; } // 编辑时使用，新增时可以为 0 或不传 

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
        // 新增时必填，编辑时如果不修改密码可以留空
        public string? Password { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        public string Role { get; set; } = "User";

        /// <summary>
        /// 账号状态
        /// </summary>
        public int Status { get; set; } = 1;
    }
}
