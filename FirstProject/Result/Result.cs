namespace FirstProject.Result
{
    /// <summary>
    /// 全局统一返回结果，统一响应体    （类似Java的Result）
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// 业务码：200成功，500业务异常
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; } = string.Empty;

        /// <summary>
        /// 返回数据（可空）
        /// </summary>
        public T? Data { get; set; }    //?代表可能为空


        // 成功且有参返回方法
        public static Result<T> Success(T everthing) { }


        // 成功且无参返回方法
        public static Result<T> Success() { }


        // 失败返回方法
        public static Result<T> Fail(String msg) { }

        internal static object? Success(object pageData)
        {
            throw new NotImplementedException();
        }
    }
}
