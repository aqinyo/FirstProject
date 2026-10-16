using FirstProject.Model.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProject.Model.Entity
{
    /// <summary>
    /// User用户表 实体类（对应数据库字段）（注：我的Model模型 → Dto、实体、Vo → 都是只有属性/字段存在，赋值操作全在Service层完成，Dto还会有个入参的非空校验）
    /// </summary>
    [Table("user")]     // 指定数据库表名为 user，这样 EF Core 才会将类名 User 转换为 user（小写），EF Core 默认是不会仅因类名是 User 就自动将表名转成小写 user 的，下面字段同理
    public class User
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称（可以通过这个来指定映射表的字段名[Column("target_user_id")]）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 角色（用户/管理员）
        /// </summary>
        public string Role { get; set; }                            

        /// <summary>
        /// 账号状态（启用/禁用、1/0）（类型是UpdateUserDto中的枚举类型）
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }             

    }
}
