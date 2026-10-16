namespace FirstProject.Model.Dto
{
    /// <summary>
    /// 动态分页查询条件（带?因为多条件查询允许部分为空）
    /// </summary>
    public class UserQueryDto : PageQueryDto
    {
        /// <summary>
        /// 关键词：匹配名称、账号或用户 ID
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 根据账号查询的入参字段
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 根据名称查询的入参字段
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 根据状态查询的入参字段
        /// </summary>
        public AccountStatus? Status { get; set; }

        /// <summary>
        /// 根据角色查询的入参字段
        /// </summary>
        public string? Role { get; set; }

    }

}
