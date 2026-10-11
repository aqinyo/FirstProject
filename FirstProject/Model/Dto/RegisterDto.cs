using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 注册入参
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public required string Name { get; set; }
        //DTO 优先 required + [Required]；实体类不建议加 required 属性,影响EF core ,[Required]根据数据库的字段是否非空而加
        //[Required]这是运行时检查，[Required] 相当于 Java 里的 @NotNull，它配合 [ApiController] 生效，如果前端传的 JSON 缺少这个字段，直接返回 400 错误

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
