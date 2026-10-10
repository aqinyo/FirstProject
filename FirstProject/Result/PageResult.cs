namespace FirstProject.Result
{
    public class PageResult<T>
    {
        public int Total { get; set; } // 总记录数

        public int Page { get; set; } // 当前页码

        public int PageSize { get; set; } // 每页记录数

        public IEnumerable<T>? Data { get; set; } // 当前页数据

    }
}
