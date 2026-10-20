using FirstProject.Model.Entity;

namespace FirstProject.Service
{
    /// <summary>
    /// 异常日志接口
    /// </summary>
    public interface IExceptionLogService
    {
        /// <summary>
        /// 插入异常日志
        /// </summary>
        /// <param name="log"></param>
        void InsertExceptionLog(ExceptionLog log);

    }

}
