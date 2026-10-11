namespace FirstProject.Result
{
    /// <summary>
    /// 全局统一返回结果，统一响应体    （类似Java的Result）
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// 业务码
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
        public static Result<T> Success(T data)
        {
            var result = new Result<T>
            {
                Code = 200,
                Msg = "成功",
                Data = data     //携带Service层返回的VO类的数据
            }; 
            return result;

        }


        // 成功且无参返回方法
        public static Result<T> Success()
        {
            var result = new Result<T>
            {
                Code = 200,
                Msg = "成功",
            }; 
            return result;   
        }


        // 失败返回方法
        public static Result<T> Fail(string msg)    // 入参是string类型的 msg，是因为Controller层调用失败方法时，直接在Controller层传入失败信息即可
        {
            var Fail = new Result<T>
            {
                Code = 500,
                Msg = msg,
            }; 
            return Fail;
        }
    }
}
