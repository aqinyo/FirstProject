using FirstProject.Data;
using FirstProject.Model.Dto;
using FirstProject.Model.Entity;
using FirstProject.Result;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirstProject.Repository
{
    /// <summary>
    /// DL层操作数据库
    /// </summary>
    public class UserRepository
    {

        private readonly AppDbContext m_context;             // 依赖注入的数据库上下文对象，用于操作数据库的 （readonly ：只能在构造函数里面赋值一次）
        /// <summary>
        /// 构造函数：依赖注入“上述所依赖注入的东西”进入容器 ，并负责创建并传入这些对象
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(AppDbContext context)
        {
            //表示把 context 赋给 m_context （_是一种常见的 C# 命名约定，用来标记私有字段）（公司规范：私有字段 + m_  全局变量+ g_ ）（全局替换可以，一开始右键字段名→重命名）
            m_context = context;  
        }



        /// <summary>
        /// 登录（返回值类型带?，可能为空，即账号可能不存在）
        /// </summary>
        /// <returns></returns>
        public User? Login(LoginDto loginDto)
        {
            User? user = m_context.User.FirstOrDefault(u => u.Account == loginDto.Account);   // 查询数据库校验账号
            return user;                                                                      // 直接返回实体类即可，Service层再做业务逻辑的检验
        }



        /// <summary>
        /// 检查账号是否已存在
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>返回类型采用bool，账号已存在返回 true，否则返回 false</returns>
        public bool AccountExists(string account)
        {
            m_context.User.Any(u => u.Account == account);

            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }



        /// <summary>
        /// 注册用户并保存到数据库
        /// </summary>
        /// <param name="user">待注册的用户实体</param>
        public bool Register(User user)
        {
            m_context.User.Add(user);

            // 原本是void 的，脚本都换成bool类型，要来验证的
            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }


        /// <summary>
        /// 根据用户 ID 查询用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns>查询到的用户实体；不存在时返回 null</returns>
        public User? GetUserById(int id)
        {
            User? user = m_context.User.Find(id);
            return user;
        }



        /// <summary>
        /// 分页查询用户实体列表
        /// </summary>
        /// <param name="pageQueryDto">分页查询条件PageQueryDto</param>
        /// <returns>包含分页信息和用户实体列表的结果</returns>
        public PageResult<User> GetUserPageList(PageQueryDto pageQueryDto)
        {
            var query = m_context.User.AsQueryable();

            int total = query.Count();
            int page = pageQueryDto.Page > 0 ? pageQueryDto.Page : 1;
            int pageSize = pageQueryDto.PageSize > 0 ? pageQueryDto.PageSize : 10;
            var data = query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // new 出返回类对象，然后把上面的值赋值给返回类对象中返回给service层
            PageResult<User> pageResult = new PageResult<User>();
            pageResult.Total = total;
            pageResult.Page = page;
            pageResult.PageSize = pageSize;
            pageResult.Data = data;

            return pageResult;
        }



        /// <summary>
        /// 动态多条件分页查询用户（按账号/名称/状态/角色查询）
        /// </summary>
        /// <param name="queryDto">用户筛选及分页条件</param>
        /// <returns>包含分页信息和符合条件的用户实体列表</returns>
        public PageResult<User> GetUserPageQueryList(UserQueryDto queryDto)
        {
            var query = m_context.User.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryDto.Keyword))
            {
                var keyword = queryDto.Keyword.Trim();
                var hasId = int.TryParse(keyword, out var id);
                query = hasId
                    ? query.Where(user => user.Id == id)
                    : query.Where(user => user.Account == keyword || user.Name == keyword);
            }

            // 多条件查询逻辑
            if (!string.IsNullOrWhiteSpace(queryDto.Account))
            {
                var account = queryDto.Account.Trim();
                query = query.Where(user => user.Account == account);
            }
            if (!string.IsNullOrWhiteSpace(queryDto.Name))
            {
                var name = queryDto.Name.Trim();
                query = query.Where(user => user.Name == name);
            }
            if (queryDto.Status.HasValue)
            {
                var status = queryDto.Status.Value;
                query = query.Where(user => user.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(queryDto.Role))
            {
                var role = queryDto.Role.Trim();
                query = query.Where(user => user.Role == role);
            }

            var total = query.Count();
            var page = Math.Max(queryDto.Page, 1);
            var pageSize = Math.Clamp(queryDto.PageSize, 1, 100);
            var userQueryData = query
                .OrderBy(user => user.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // new 出返回类对象，然后把上面的值赋值给返回类对象中返回给service层
            PageResult<User> userPageResultList = new PageResult<User>();
            userPageResultList.Total = total;
            userPageResultList.Page = page;
            userPageResultList.PageSize = pageSize;
            userPageResultList.Data = userQueryData;

            return userPageResultList;
        }



        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user">待新增的用户实体</param>
        public void AddUser(User user)
        {
            m_context.User.Add(user);
            m_context.SaveChanges();
        }



        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user">包含待更新信息的用户实体</param>
        /// <returns>数据库受影响行数大于 0 时返回 true，否则返回 false</returns>
        public bool UpdateUser(User user)
        {
            User? existingUser = m_context.User.Find(user.Id);   // 允许可能为 null ，并加防空判断  【涉及到可能为空的情况，带?的，基本都要加一个防空判断的防御性编程】
            if (existingUser is null)
            {
                return false;                                   // 不存在的话就返回false，由我的service抛出对应的业务异常，Controller再转换为对应的code、msg
            }

            // 把查询出来原本的User，修改对应字段
            existingUser.Name = user.Name;
            existingUser.Account = user.Account;
            existingUser.Role = user.Role;
            existingUser.Status = user.Status;

            // 原本是void 的，脚本都换成bool类型，要来验证的
            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户 ID（入参必须要有id，否则不知道操作哪个）</param>
        /// <param name="newPassword">新密码密文</param>
        /// <returns>只需要返回是否更新成功即可，所以是bool类型返回值</returns>
        public bool UpdatePassword(int userId, string newPassword)
        {
            User? user = m_context.User.Find(userId);
            if (user is null)
            {
                return false;
            }

            // 修改密码
            user.Password = newPassword;

            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }



        /// <summary>
        /// 删除用户（根据ID）
        /// </summary>
        /// <param name="id">待删除用户的 ID</param>
        public bool DeleteUser(int id)
        {
            User? deleteUser = m_context.User.Find(id);
            if (deleteUser is null)
            {
                return false;
            }

            m_context.User.Remove(deleteUser);

            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }



        /// <summary>
        /// 启用禁用账号状态（根据用户 ID） 
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <param name="operatorId">操作人ID</param>
        /// <param name="operatorName">操作人Name</param>
        /// <param name="reason">禁用原因</param>
        public bool EnableOrDisableAccountStatus(int id, int operatorId, string operatorName, string reason)
        {
            User? manageAccount = m_context.User.Find(id);   // 按主键查询出整个User，再通过manageAccount.Status提取出status来修改即可
            if (manageAccount is null)                       // 防御性编程
            {
                return false;
            }

            // 判断从数据库查询出来的状态是否为“禁用”
            AccountStatus beforeStatus = manageAccount.Status;
            AccountStatus afterStatus = beforeStatus == AccountStatus.Enabled? AccountStatus.Disabled: AccountStatus.Enabled;   // 若beforeStatus == AccountStatus.Enabled?为真，赋第一个值，为假则第二个
            manageAccount.Status = afterStatus;

            // new 出来 + 手动赋值
            StatusLog statusLog = new StatusLog();
            statusLog.TargetUserId = manageAccount.Id;
            statusLog.OperatorId = operatorId;
            statusLog.OperatorName = operatorName;
            statusLog.BeforeStatus = (int)beforeStatus;
            statusLog.AfterStatus = (int)afterStatus;
            statusLog.Reason = reason;
            statusLog.OperatorTime = DateTime.Now;

            // 插入日志表
            m_context.StatusLog.Add(statusLog);

            // 原本是void 的，脚本都换成bool类型，要来验证的
            var rowsAffected = m_context.SaveChanges();
            return rowsAffected > 0;
        }

    }
}
