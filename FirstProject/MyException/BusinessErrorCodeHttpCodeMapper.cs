namespace FirstProject.MyException

{
    /// <summary>
    /// 自定义的BusinessErrorCode业务码  映射为  标准Http状态码   的映射规则（抽取出来后，每个Controller都可以使用了）
    /// </summary>
    public class BusinessErrorCodeHttpCodeMapper
    {

        /// <summary>
        /// 公共映射规则（给Controller调用映射规则的）
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static int Mapper(BusinessErrorCode errorCode)
        {

            int httpCode = errorCode switch
            {
                // 自定义业务码 AccountAlreadyExists（1001）   —映射为→   HTTP 409
                BusinessErrorCode.AccountAlreadyExists => StatusCodes.Status409Conflict,

                // 自定义业务码InvalidCredentials （1002）   —映射为→   HTTP 401
                BusinessErrorCode.InvalidCredentials => StatusCodes.Status401Unauthorized,

                // 自定义业务码AccountDisabled （1003）   —映射为→   HTTP 403
                BusinessErrorCode.AccountDisabled => StatusCodes.Status403Forbidden,

                // 自定义业务码UserNotFound （1004）   —映射为→   HTTP 404
                BusinessErrorCode.UserNotFound => StatusCodes.Status404NotFound,

                // 自定义业务码NewPasswordMismatch（1005）   —映射为→   HTTP 400
                BusinessErrorCode.NewPasswordMismatch => StatusCodes.Status400BadRequest,

                // 自定义业务码PasswordTooShort（1006）   —映射为→   HTTP 400
                BusinessErrorCode.PasswordTooShort => StatusCodes.Status400BadRequest,

                // 自定义业务码InvalidUserIdentity （1007）   —映射为→   HTTP 401
                BusinessErrorCode.InvalidUserIdentity => StatusCodes.Status401Unauthorized,

                // 自定义业务码StatusChangeReasonRequired（1008） —映射为→ HTTP 400
                BusinessErrorCode.StatusChangeReasonRequired => StatusCodes.Status400BadRequest,

                // 自定义业务码StatusChangeReasonRequired（1009） —映射为→ HTTP 401
                BusinessErrorCode.InvalidPasswords => StatusCodes.Status401Unauthorized,


                // 其余的/未被定义的异常 统一映射为→ HTTP 500
                _ => StatusCodes.Status500InternalServerError

            };

            return httpCode;

        }


    }

}
