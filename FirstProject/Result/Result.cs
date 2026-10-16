namespace FirstProject.Result
{
    /// <summary>
    /// 全局统一返回结果，统一响应体    （类似Java的Result）
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// HTTP状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; } = string.Empty;

        /// <summary>
        /// 返回数据（?代表可为空）
        /// </summary>
        public T? Data { get; set; }  



        /// <summary>
        /// 成功返回方法（有参）
        /// </summary>
        /// <param name="data">有参使用入参的是数据（成功的返回结果集状态码和提示信息固定为“200和成功”）</param>
        /// <returns></returns>
        public static Result<T> Success(T data)
        {
            var success = new Result<T>();

            success.Code = 200;
            success.Msg = "成功"; 
            success.Data = data;     // 携带Service层返回的VO类的数据

            return success;

        }

        

        /// <summary>
        /// 成功返回方法（无参）
        /// </summary>
        /// <returns></returns>
        public static Result<T> Success()
        {
            var success = new Result<T>();

            success.Code = 200;
            success.Msg = "成功";

            return success;   
        }



        /// <summary>
        /// 失败返回方法（仅返回 msg ，状态码固定500）
        /// </summary>
        /// <param name="msg">仅错误信息</param>
        /// <returns></returns>
        public static Result<T> Fail(string msg)    // 入参是string类型的 msg，是因为Controller层调用失败方法时，直接在Controller层传入失败信息即可
        {
            var fail = new Result<T>();

            fail.Code = 500;
            fail.Msg = msg;

            return fail;
        }



        /// <summary>
        /// 失败返回方法（返回 code + msg ）
        /// </summary>
        /// <param name="code">响应码</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public static Result<T> Fail(int code, string msg)    // 入参是string类型的 msg，是因为Controller层调用失败方法时，直接在Controller层传入失败信息即可
        {
            var fail = new Result<T>();

            fail.Code = code;
            fail.Msg = msg;

            return fail;
        }


    }
}
