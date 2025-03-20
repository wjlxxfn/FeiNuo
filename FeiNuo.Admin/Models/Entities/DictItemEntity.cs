using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：字典项
/// </summary>
public partial class DictItemEntity : BaseEntity
{
    /// <summary>
    /// 字典项主键
    /// </summary>
    public int DictItemId { get; set; }

    /// <summary>
    /// 字典主键
    /// </summary>
    public int DictId { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    public string DictLabel { get; set; } = null!;

    /// <summary>
    /// 字典键值
    /// </summary>
    public string DictValue { get; set; } = null!;

    /// <summary>
    /// 其他配置
    /// </summary>
    public string? ExtValue { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public short SortNo { get; set; }

    /// <summary>
    /// 字典状态
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    public virtual DictEntity Dict { get; set; } = null!;
}

/// <summary>
/// 数据库配置：字典项
/// </summary>
public partial class DictItemConfiguration : IEntityTypeConfiguration<DictItemEntity>
{
    public void Configure(EntityTypeBuilder<DictItemEntity> entity)
    {
        entity.HasKey(e => e.DictItemId).HasName("pk_sys_dict_item");

        entity.ToTable("sys_dict_item", tb => tb.HasComment("字典项"));

        entity.HasIndex(e => e.DictId, "ix_sys_dict_item_dict_id");

        entity.Property(e => e.DictItemId).HasColumnName("dict_item_id").HasComment("字典项主键");
        entity.Property(e => e.DictId).HasColumnName("dict_id").HasComment("字典主键");
        entity.Property(e => e.DictLabel).HasColumnName("dict_label").HasComment("字典标签").HasMaxLength(50);
        entity.Property(e => e.DictValue).HasColumnName("dict_value").HasComment("字典键值").HasMaxLength(200).IsUnicode(false);
        entity.Property(e => e.ExtValue).HasColumnName("ext_value").HasComment("其他配置").HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.SortNo).HasColumnName("sort_no").HasComment("排序序号");
        entity.Property(e => e.Disabled).HasColumnName("disabled").HasComment("字典状态");
        entity.Property(e => e.Remark).HasComment("备注说明").HasMaxLength(200);

        entity.HasOne(d => d.Dict).WithMany(p => p.DictItems)
            .HasForeignKey(d => d.DictId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_sys_dict_item_dict_id");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DictItemEntity> modelBuilder);
}
