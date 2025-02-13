using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：参数配置
/// </summary>
public partial class ConfigEntity : BaseEntity
{
    /// <summary>
    /// 参数配置ID
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// 参数编码
    /// </summary>
    public string ConfigCode { get; set; } = null!;

    /// <summary>
    /// 参数名称
    /// </summary>
    public string ConfigName { get; set; } = null!;

    /// <summary>
    /// 配置内容
    /// </summary>
    public string ConfigValue { get; set; } = null!;

    /// <summary>
    /// 其他配置
    /// </summary>
    public string? ExtraValue { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 数据库配置：参数配置
/// </summary>
public partial class ConfigConfiguration : IEntityTypeConfiguration<ConfigEntity>
{
    public void Configure(EntityTypeBuilder<ConfigEntity> entity)
    {
        entity.HasKey(e => e.ConfigId).HasName("pk_sys_config");

        entity.ToTable("sys_config", tb => tb.HasComment("参数配置"));

        entity.HasIndex(e => e.ConfigCode, "uk_sys_config_config_key").IsUnique();

        entity.Property(e => e.ConfigId).HasColumnName("config_id").HasComment("参数配置ID");
        entity.Property(e => e.ConfigCode).HasColumnName("config_code").HasComment("参数编码").HasMaxLength(100).IsUnicode(false);
        entity.Property(e => e.ConfigName).HasColumnName("config_name").HasComment("参数名称").HasMaxLength(50);
        entity.Property(e => e.ConfigValue).HasColumnName("config_value").HasComment("配置内容").HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.ExtraValue).HasColumnName("extra_value").HasComment("其他配置").HasDefaultValue("").HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注说明").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ConfigEntity> modelBuilder);
}
