using FirstProject.Model.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProject.Model.Entity
{
    [Table("user")] // 指定数据库表名为 user，EF Core 默认会将类名 User 转换为 user（小写）
    public class User
    {
        public int Id { get; set; }// 主键，EF Core 默认识别 Id 为主键    【注：与Java不同，C#的model的属性大部分是public】
         
        [Required]  //因为数据库层面是非空的，所以实体类这里要加[Required]，用来生成数据库的 NOT NULL 约束,但是不加 required 属性，影响到 EF core框架
        public string Name { get; set; } = string.Empty;// 非空

        [Required]
        public string Account { get; set; } = string.Empty;// 非空

        [Required]
        public string Password { get; set; } = string.Empty;// 非空

        [Required]
        public string Role { get; set; } = "User";// 默认用户，管理员为Admin

        public AccountStatus Status { get; set; } = AccountStatus.Enabled;  //默认启用状态（1），禁用状态为0（采用枚举，只能是0或1，避免前端传其他数字）

        public DateTime CreateTime { get; set; } = DateTime.Now;// 默认当前时间
    }
}
