using FeiNuo.Admin.Models;
using FeiNuo.Core.Login;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services
{
    public class LoginService : BaseService, ILoginUserService
    {
        private readonly FNDbContext ctx;

        public LoginService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<LoginUser?> LoadUserByUsername(string username)
        {
            // 查询用户角色
            var user = await ctx.Users.Include(a => a.Dept).AsNoTracking()
                .Select(a => new
                {
                    a.UserId,
                    a.Username,
                    a.Password,
                    a.Nickname,
                    a.Dept.DeptId,
                    a.Dept.DeptName,
                }).SingleOrDefaultAsync(a => a.Username == username);

            if (null == user) return null;

            // 查询角色/权限
            var userRoles = await ctx.Roles
                .FromSql($"SELECT r.role_id,r.role_code FROM sys_role r JOIN sys_user_role ur ON r.role_id = ur.role_id where ur.user_id = {user.UserId}")
                .Include(a => a.Menus)
                .Select(a => new { a.RoleCode, Permission = a.Menus.Select(a => a.Permission) })
                .ToListAsync();
            var roles = userRoles.Select(a => a.RoleCode);
            var permissions = Array.Empty<string>();// userRoles.SelectMany(a => a.Permission).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();

            var data = user.UserId + "," + user.DeptId + "," + user.DeptName;
            return new LoginUser(user.Username, user.Nickname, user.Password, roles, permissions, data);
        }

        /// <summary>
        /// 登录成功后，返回前端的用户信息
        /// </summary>
        public async Task<Dictionary<string, object>> GetLoginUserInfo(LoginUser user)
        {
            var map = new Dictionary<string, object>
            {
                { "username", user.Username },
                { "nickname", user.Nickname },
                { "roles", user.Roles },
                { "permissions", user.Permissions },
            };
            var u = await ctx.FindAsync<UserEntity>(user.GetUserData().UserId);
            map.Add("avatar", u?.Avatar ?? "");

            // 菜单一起返回，不再单独查一次
            var menuQuery = user.IsSuperAdmin
                ? ctx.Menus.AsQueryable()
                : ctx.Menus.Where(m => m.Roles.Any(r => user.Roles.Contains(r.RoleCode)));

            // 这里不加AsNoTracking,查出的结果会自动关联parent,children
            var menus = await menuQuery.Where(a => a.MenuType != ((int)MenuTypeEnum.Button)).OrderBy(m => m.SortNo).ToListAsync();

            // 转成菜单
            var menuVOs = ConvertToMenuVO(menus.Where(a => a.Parent == null));
            map.Add("menus", menuVOs);

            return map;
        }
        private static List<MenuVO> ConvertToMenuVO(IEnumerable<MenuEntity> menus)
        {
            return [.. menus.Select(a => new MenuVO(a.MenuName, a.MenuPath, a.Icon)
            {
                Children = ConvertToMenuVO(a.Children)
            })];
        }

        //private static List<RouteVO> ConvertMenuToRoute(IEnumerable<MenuEntity> menus)
        //{
        //    var routes = new List<RouteVO>();
        //    foreach (var menu in menus)
        //    {
        //        var route = new RouteVO
        //        {
        //            Name = string.IsNullOrWhiteSpace(menu.Component) ? "" : menu.MenuPath.TrimStart('/').ToUpperFirst(),
        //            Component = menu.Component,
        //            Path = menu.MenuPath,
        //            //TODO 模块要不要加个redirect到具体的菜单
        //            Meta = new MetaVO(menu.MenuName, menu.Icon, menu.Hidden, menu.NoCache),
        //            Children = ConvertMenuToRoute(menu.Children),
        //        };
        //        routes.Add(route);
        //    }
        //    return routes;
        //}
    }
}
