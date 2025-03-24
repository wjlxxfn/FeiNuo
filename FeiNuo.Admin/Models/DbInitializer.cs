namespace FeiNuo.Admin.Models;

public class DbInitializer
{
    /// <summary>
    /// 自动创建数据库
    /// </summary>
    public static void EnsureDatabaseCreated(IServiceProvider serviceProvider, bool recreated = false)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<FNDbContext>();
        if (recreated)
        {
            ctx.Database.EnsureDeleted();
        }
        ctx.Database.EnsureCreated();

        EnsureSeedData(ctx);
    }

    /// <summary>
    /// 生成初始化数据
    /// </summary>
    /// <param name="ctx"></param>
    private static void EnsureSeedData(FNDbContext ctx)
    {
        if (ctx.Users.Any()) return;

        using var trans = ctx.Database.BeginTransaction();

        #region 部门数据
        var dept = NewDept("Top平台", 0, [
            NewDept("研发单位", 1, [
                NewDept("研发部门", 1, [NewDept("JAVA项目组", 1), NewDept("C#项目组", 2), NewDept("前端项目组", 3), NewDept("移动APP项目组", 4),]),
                NewDept("测试部门", 2, [NewDept("黑盒测试", 1), NewDept("白盒测试", 2), NewDept("自动化测试", 3)]),
                NewDept("其他部门", 3, [NewDept("背锅专用", 1),]),
            ]),
            NewDept("外部单位", 2, [NewDept("外部XX1",1), NewDept("外部XXX2",2) ]),
        ]);
        ctx.Depts.Add(dept);
        #endregion

        #region 菜单数据 
        List<MenuEntity> menus = [
            NewMenu(MenuTypeEnum.Module, "系统管理", "system",1,"",[
                NewMenu(MenuTypeEnum.Menu, "用户管理", "system.user", 1),
                NewMenu(MenuTypeEnum.Menu, "角色管理", "system.role", 2),
                NewMenu(MenuTypeEnum.Menu, "菜单管理", "system.menu", 3),
                NewMenu(MenuTypeEnum.Menu, "部门管理", "system.dept", 4),
                NewMenu(MenuTypeEnum.Menu, "字典管理", "system.dict", 5),
                NewMenu(MenuTypeEnum.Menu, "参数配置", "system.config", 6),
            ]),

            NewMenu(MenuTypeEnum.Module, "系统监控","monitor", 2,"",[
                NewMenu(MenuTypeEnum.Menu, "日志管理","monitor.log",3),
                NewMenu(MenuTypeEnum.Menu, "系统状态","monitor.state",3),
            ]),

            NewMenu(MenuTypeEnum.Module, "嵌套菜单","nested",5,"",[
                NewMenu(MenuTypeEnum.Module, "第一层模块","nested.first-module",1,"",[
                    NewMenu(MenuTypeEnum.Module, "第二层模块","nested.second-module",4,"",[
                        NewMenu(MenuTypeEnum.Menu, "第三层菜单","nested.thirdmenu",4),
                        NewMenu(MenuTypeEnum.Menu, "第三层菜单","nested.thirdmenu2",4),
                    ]),
                    NewMenu(MenuTypeEnum.Menu, "第二层菜单","nested.second-menu",4),
                ]),
                NewMenu(MenuTypeEnum.Menu, "第一层菜单","nested.first-menu",2),
            ]),

            NewMenu(MenuTypeEnum.Menu, "官网链接","http://www.xxfn.top",4, ""),
        ];
        ctx.Menus.AddRange(menus);
        ctx.SaveChanges();
        #endregion

        #region 角色数据
        menus = [.. ctx.Menus];
        var roleSuper = NewRole("SuperAdmin", "超级管理员", "超级管理员，拥有所有权限，系统内置，不允许编辑删除", menus);
        var roleAdmin = NewRole("admin", "管理员", "普通管理员", menus.Where(a => !a.Permission.StartsWith("monitor")));
        var roleWjl = NewRole("wjl", "wjl", "wjl专用角色", menus.Where(a => a.Permission == "" || a.Permission.StartsWith("system")));
        var roleTest = NewRole("test", "test", "测试用");
        ctx.Roles.AddRange(roleSuper, roleAdmin, roleWjl, roleTest);
        #endregion

        #region 用户数据
        ctx.Users.AddRange(
            NewUser("SuperAdmin", "超级管理员", [roleSuper]),
            NewUser("admin", "管理员", [roleAdmin]),
            NewUser("wjl", "wjl", [roleWjl, roleAdmin]),
            NewUser("test", "test", [roleTest])
         );
        ctx.SaveChanges();
        #endregion

        #region 配置数据
        ctx.Configs.AddRange(
           Audit(new ConfigEntity()
           {
               ConfigCode = "system.init_password",
               ConfigName = "初始密码",
               ConfigValue = "123456",
               Remark = "新增用户的初始密码",
               ExtraValue = ""
           })
        );
        ctx.SaveChanges();
        #endregion

        #region 数据字典
        ctx.Dicts.AddRange(Audit(new DictEntity()
        {
            DictType = "UserGender",
            DictName = "性别",
            DictItems = [
                Audit(new DictItemEntity() { DictValue = "M", DictLabel = "男" }),
                Audit(new DictItemEntity() { DictValue = "F", DictLabel = "女" }),
                Audit(new DictItemEntity() { DictValue = "O", DictLabel = "保密" }),
            ]
        }));
        ctx.SaveChanges();
        #endregion

        trans.Commit();
    }

    #region 辅助方法
    private static MenuEntity NewMenu(MenuTypeEnum type, string name, string code, int sortNo, string icon = "", List<MenuEntity>? children = null)
    {
        string path = type == MenuTypeEnum.Button ? "" : code.StartsWith("http") ? code : "/" + code.Replace(".", "/");
        string permission = code.StartsWith("http") ? "" : code.Replace(".", ":");
        var menu = new MenuEntity
        {
            MenuType = (int)type,
            MenuName = name,
            MenuPath = path,
            Permission = permission,
            SortNo = (short)sortNo,
            Children = children ?? []
        };
        //if (type == MenuTypeEnum.Menu && permission != "")
        //{
        //    menu.Children.Add(NewMenu(MenuTypeEnum.Button, "新增", $"{permission}.create", 1));
        //    menu.Children.Add(NewMenu(MenuTypeEnum.Button, "编辑", $"{permission}.update", 2));
        //    menu.Children.Add(NewMenu(MenuTypeEnum.Button, "删除", $"{permission}.delete", 3));
        //}
        return Audit(menu);
    }
    private static DeptEntity NewDept(string deptName, int sortNo, IEnumerable<DeptEntity>? children = null)
    {
        var dept = new DeptEntity()
        {
            DeptName = deptName,
            SortNo = (short)sortNo,
        };
        if (children != null)
        {
            dept.Children = [.. children];
        }
        return Audit(dept);
    }
    private static RoleEntity NewRole(string roleCode, string roleName, string remark, IEnumerable<MenuEntity>? menus = null)
    {
        var role = new RoleEntity
        {
            RoleCode = roleCode,
            RoleName = roleName,
            Remark = remark,
            Menus = [.. menus ?? []]
        };
        return Audit(role);
    }
    private static UserEntity NewUser(string username, string nickname, List<RoleEntity> roles)
    {
        var pwd = "123456";
        var avatar = "https://s2.loli.net/2022/04/07/gw1L2Z5sPtS8GIl.gif";
        var user = new UserEntity()
        {
            Roles = roles,
            Username = username,
            Password = pwd,
            DeptId = 1,
            Nickname = nickname,
            Cellphone = "16666666666",
            Gender = "O",
            Email = "wjlxxfn@gmail.com",
            Avatar = avatar
        };
        return Audit(user);
    }
    private static T Audit<T>(T entity) where T : BaseEntity
    {
        entity.CreateBy = entity.UpdateBy = "SuperAdmin";
        entity.CreateTime = entity.UpdateTime = DateTime.Now;
        return entity;
    }
    #endregion
}