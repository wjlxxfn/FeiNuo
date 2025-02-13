using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Models;

public partial class FNDbContext
{
    // 全局配置
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.Properties<string>().AreUnicode(false);
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

        // 批量覆盖配置
        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    foreach (var propertyInfo in entityType.ClrType.GetProperties())
        //    {
        //        if (propertyInfo.Name.ToLower() == "remark" || propertyInfo.Name.ToLower() == "description" || propertyInfo.Name.EndsWith("name"))
        //        {
        //            entityType.FindProperty(propertyInfo.Name)?.SetIsUnicode(true);
        //        }
        //    }
        //}
    }
}
