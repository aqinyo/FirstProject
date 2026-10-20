using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 状态日志表的入参Dto
    /// </summary>
    public class StatusLogDto
    {

        /// <summary>
        /// 修改原因（禁用时必填，启用时可为空）
        /// </summary>
        [MaxLength(200, ErrorMessage = "禁用原因不能超过200个字符")]
        public string? Reason { get; set; }

    }
}
