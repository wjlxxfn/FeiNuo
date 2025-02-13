using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Models;

public partial class FNDbContext : DbContext
{
    public FNDbContext(DbContextOptions<FNDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 参数配置
    /// </summary>
    public virtual DbSet<ConfigEntity> Configs { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public virtual DbSet<DeptEntity> Depts { get; set; }

    /// <summary>
    /// 操作日志
    /// </summary>
    public virtual DbSet<LogEntity> Logs { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    public virtual DbSet<MenuEntity> Menus { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public virtual DbSet<RoleEntity> Roles { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    public virtual DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 参数配置
        modelBuilder.ApplyConfiguration(new ConfigConfiguration());
        // 部门
        modelBuilder.ApplyConfiguration(new DeptConfiguration());
        // 操作日志
        modelBuilder.ApplyConfiguration(new LogConfiguration());
        // 菜单
        modelBuilder.ApplyConfiguration(new MenuConfiguration());
        // 角色
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        // 用户
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
