using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProject.Model.Entity
{
    /// <summary>
    /// StatusLog 状态日志表的实体类（指定数据库表名为status_log）
    /// </summary>
    [Table("status_log")]   
    public class StatusLog
    {
        /// <summary>
        /// 状态日志表主键id   
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 被修改人id  （可以通过[Column("target_user_id")]来指定映射表的字段名）（前端把被操作人的当前id传给后端）
        /// </summary>
        [Column("target_user_id")]
        public int TargetUserId { get; set; }

        /// <summary>
        /// 修改人id
        /// </summary>
        [Column("operator_id")]
        public int OperatorId { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        [Column("operator_name")]
        public string OperatorName { get; set; }

        /// <summary>
        /// 修改前状态（可为空，其余not null）
        /// </summary>
        [Column("before_status")]
        public int BeforeStatus { get; set; }

        /// <summary>
        /// 修改后状态
        /// </summary>
        [Column("after_status")]
        public int AfterStatus { get; set; }

        /// <summary>
        /// 修改原因（启用禁用原因）（唯一的Dto输入字段）
        /// </summary>
        [Column("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("operator_time")]
        public DateTime OperatorTime { get; set; }

    }
}
