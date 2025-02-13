namespace FeiNuo.Core
{
    public static class LoginUserExtensions
    {
        public static UserData GetUserData(this LoginUser user)
        {
            var data = user.UserData.Split(',');
            var userId = int.Parse(data[0]);
            var deptId = int.Parse(data[1]);
            return new UserData(userId, deptId, data[2]);
        }
    }

    public class UserData
    {
        public int UserId { get; private set; }

        public int DeptId { get; private set; }

        public string DeptName { get; private set; }

        public UserData(int userId, int deptId, string deptName)
        {
            UserId = userId;
            DeptId = deptId;
            DeptName = deptName;
        }
    }
}
