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
        /// 修改原因（启用禁用原因）(必填，所以加了非空校验)
        /// </summary>
        [Required]
        public required string Reason { get; set; }

    }
}
