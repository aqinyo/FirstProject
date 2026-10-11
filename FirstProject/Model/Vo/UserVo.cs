using FirstProject.Model.Dto;

namespace FirstProject.Model.VO
{
    public class UserVo
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Account { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public AccountStatus Status { get; set; } = AccountStatus.Enabled;

        public DateTime CreateTime { get; set; }//CreateTime 不需要 = DateTime.Now，因为它是从数据库查出来的，EF Core 会自动赋值
    }
}
