using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Models;

public partial class FNDbContext
{
    // 全局配置
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // pgsql 时间戳类型
        // configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp");
        configurationBuilder.Properties<string>().AreUnicode(false);
        configurationBuilder.Properties<char>().AreUnicode(false);
    }

    /// <summary>
    /// 覆盖自动生成的数据库配置
    /// </summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // 自动加载关联数据
        modelBuilder.Entity<UserEntity>().Navigation(a => a.Dept).AutoInclude();

        // 日志不会有修改记录，去掉几个公用字段
        modelBuilder.Entity<LogEntity>().Ignore(a => a.UpdateBy).Ignore(a => a.UpdateTime);

        modelBuilder.Entity<DictItemEntity>().Ignore(a => a.CreateBy).Ignore(a => a.CreateTime).Ignore(a => a.UpdateBy).Ignore(a => a.UpdateTime);

        // 批量覆盖配置
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var propertyInfo in entityType.ClrType.GetProperties())
            {
                var p = propertyInfo.Name.ToLower();
                if (p.EndsWith("remark") || p == "description" || p == "introduction" || p.EndsWith("name"))
                {
                    entityType.FindProperty(propertyInfo.Name)?.SetIsUnicode(true);
                }
            }
        }
    }
}
