namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 分页查询入参Dto
    /// </summary>
    public class PageQueryDto
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; } = 1;          // 默认第一页


        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; } = 10;     // 默认每页10条记录
    }
}
