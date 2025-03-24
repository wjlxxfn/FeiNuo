using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：数据字典
/// </summary>
public partial class DictEntity : BaseEntity
{
    /// <summary>
    /// 字典主键
    /// </summary>
    public int DictId { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public string DictType { get; set; } = null!;

    /// <summary>
    /// 字典名称
    /// </summary>
    public string DictName { get; set; } = null!;

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    public virtual ICollection<DictItemEntity> DictItems { get; set; } = [];
}

/// <summary>
/// 数据库配置：数据字典
/// </summary>
public partial class DictConfiguration : IEntityTypeConfiguration<DictEntity>
{
    public void Configure(EntityTypeBuilder<DictEntity> entity)
    {
        entity.HasKey(e => e.DictId).HasName("pk_sys_dict");

        entity.ToTable("sys_dict", tb => tb.HasComment("数据字典"));

        entity.Property(e => e.DictId).HasColumnName("dict_id").HasComment("字典主键");
        entity.Property(e => e.DictType).HasColumnName("dict_type").HasComment("字典类型").HasMaxLength(50);
        entity.Property(e => e.DictName).HasColumnName("dict_name").HasComment("字典名称").HasMaxLength(50);
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注说明").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DictEntity> modelBuilder);
}
