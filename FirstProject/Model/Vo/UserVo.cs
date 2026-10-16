using FirstProject.Model.Dto;

namespace FirstProject.Model.VO
{
    /// <summary>
    /// 用户详细信息返回的Vo类（注：我的Model模型 → Dto、实体、Vo → 都是只有属性/字段存在，赋值操作全在Service层完成，Dto还会有个入参的非空校验）
    /// </summary>
    public class UserVo
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } 

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; } 

        /// <summary>
        /// 账号状态
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }    //CreateTime 不需要 = DateTime.Now，因为它是从数据库查出来的，EF Core 会自动赋值

    }
}
