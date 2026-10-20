using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProject.Model.Entity
{
    /// <summary>
    /// 异常日志表（“全局异常处理器”需要使用到）
    /// </summary>
    [Table("exception_log")]   
    public class ExceptionLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 业务模块：User/...
        /// </summary>
        [Column("business_module")]
        public string BusinessModule { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        [Column("operator_id")]
        public int? OperatorId { get; set; }

        /// <summary>
        /// 操作人Name
        /// </summary>
        [Column("operator_name")]
        public string OperatorName { get; set; }

        /// <summary>
        /// 请求接口地址
        /// </summary>
        [Column("request_url")]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求风格：GET/POST
        /// </summary>
        [Column("request_method")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求入参 JSON (Text类型)
        /// </summary>
        [Column("request_json")]
        public string RequestParam { get; set; }

        /// <summary>
        /// 异常简短信息
        /// </summary>
        [Column("exception_msg")]
        public string ExceptionMsg { get; set; }

        /// <summary>
        /// 完整的异常堆栈 (Text类型)
        /// </summary>
        [Column("exception_stack")]
        public string ExceptionStack { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; }

    }
}
