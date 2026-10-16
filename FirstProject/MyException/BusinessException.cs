namespace FirstProject.MyException
{

    /// <summary> 
    /// 业务异常业务码 类（枚举）（自定义的）（然后要在全局异常处理器中，写方法和标准的HTTP码去进行映射，这里的BusinessException只负责定义业务码和异常信息而已）
    /// </summary>
    public enum BusinessErrorCode
    {
        /// <summary>
        /// 账号已存在
        /// </summary>
        AccountAlreadyExists = 1001,

        /// <summary>
        /// 账号/密码错误
        /// </summary>
        InvalidCredentials = 1002,

        /// <summary>
        /// 账号被禁用
        /// </summary>
        AccountDisabled = 1003,

        /// <summary>
        /// 用户不存在
        /// </summary>
        UserNotFound = 1004,

        /// <summary>
        /// 两次密码不一致
        /// </summary>
        NewPasswordMismatch = 1005,

        /// <summary>
        /// 密码未达到最小长度要求
        /// </summary>
        PasswordTooShort = 1006,

        /// <summary>
        /// 登录身份无效
        /// </summary>
        InvalidUserIdentity = 1007
    }




    /// <summary>
    /// 异常信息提示常量 类
    /// </summary>
    public static class ConstantMessage
    {
        /// <summary>
        /// 1001 异常提示信息
        /// </summary>
        public const string AccountAlreadyExists = "该账号已被注册，请更换账号！";

        /// <summary>
        /// 1002 异常提示信息
        /// </summary>
        public const string InvalidCredentials = "账号或密码错误！";

        /// <summary>
        /// 1003 异常提示信息
        /// </summary>
        public const string AccountDisabled = "该账号已被禁用！禁用原因：（这时候导入要新加入的备注内容了）";

        /// <summary>
        /// 1004 异常提示信息
        /// </summary>
        public const string UserNotFound = "用户不存在！";

        /// <summary>
        /// 1005 异常提示信息
        /// </summary>
        public const string NewPasswordMismatch = "两次输入的新密码不一致！";

        /// <summary>
        /// 1006 异常提示信息
        /// </summary>
        public const string PasswordTooShort = "新密码长度不能少于6位！";

        /// <summary>
        /// 1007 异常提示信息
        /// </summary>
        public const string InvalidUserIdentity = "登录身份无效！";

    }       
     



    /// <summary>
    /// 自定义的业务异常 类（Controller内的catch()中，捕获的异常类就是这个类，负责携带上面的“业务码 + 异常提示信息”）
    /// </summary>
    public sealed class BusinessException : Exception
    {
        /// <summary>
        /// 业务码属性
        /// </summary>
        public BusinessErrorCode ErrorCode { get; }         // 这个 ErrorCode 业务码就是属于上面自定义的类（业务异常业务码 类）


        /// <summary>
        /// Service抛出的异常就是这个异常类（入参有业务码和异常信息）
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public BusinessException(BusinessErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

    }



}
