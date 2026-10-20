using FirstProject.Data;
using FirstProject.Model.Entity;

namespace FirstProject.Service
{
    /// <summary>
    /// 异常日志方法实现类
    /// </summary>
    public class ExceptionLogService : IExceptionLogService
    {

        private readonly AppDbContext m_context;             // 依赖注入的数据库上下文对象，用于操作数据库的 （readonly ：只能在构造函数里面赋值一次）
        /// <summary>
        /// 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        /// </summary>
        /// <param name="context"></param>
        public ExceptionLogService(AppDbContext context)
        {
            m_context = context;
        }


        /// <summary>
        /// 实现“写入异常日志”的接口方法
        /// </summary>
        /// <param name="log"></param>
        public void InsertExceptionLog(ExceptionLog log)
        {
            m_context.ExceptionLog.Add(log);
            m_context.SaveChanges(); 
        }


    }
}
