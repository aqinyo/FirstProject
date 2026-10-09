using System;
using System.ComponentModel.DataAnnotations;

namespace FirstProject.Model.Entity
{
    public class User
    {
        public int Id { get; set; }// 主键，EF Core 默认识别 Id 为主键    【注：与Java不同，C#的model的属性大部分是public】
         
        [Required]  //因为数据库层面是非空的，所以实体类这里要加[Required]，用来生成数据库的 NOT NULL 约束,但是不加 required 属性，影响到 EF core框架
        public string Name { get; set; } = string.Empty;// 非空

        [Required]
        public string Account { get; set; } = string.Empty;// 非空

        [Required]
        public string Passname { get; set; } = string.Empty;// 非空

        [Required]
        public string Role { get; set; } = "User";// 默认用户，管理员为Admin

        public int Status { get; set; } = 1;//默认1，是启用状态；0是禁用状态

        public DateTime CreateTime { get; set; } = DateTime.Now;// 默认当前时间
    }
}
