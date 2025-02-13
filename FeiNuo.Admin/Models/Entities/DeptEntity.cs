using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：部门
/// </summary>
public partial class DeptEntity : FeiNuo.Core.BaseEntity
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public int DeptId { get; set; }

    /// <summary>
    /// 上级ID
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = null!;

    /// <summary>
    /// 排序号
    /// </summary>
    public short SortNo { get; set; }

    /// <summary>
    /// 是否作废
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    public virtual ICollection<DeptEntity> Children { get; set; } = [];

    public virtual DeptEntity? Parent { get; set; }

    public virtual ICollection<UserEntity> Users { get; set; } = [];
}

/// <summary>
/// 数据库配置：部门
/// </summary>
public partial class DeptConfiguration : IEntityTypeConfiguration<DeptEntity>
{
    public void Configure(EntityTypeBuilder<DeptEntity> entity)
    {
        entity.HasKey(e => e.DeptId).HasName("pk_sys_dept");

        entity.ToTable("sys_dept", tb => tb.HasComment("部门"));

        entity.HasIndex(e => e.ParentId, "ix_sys_dept_parent_id");

        entity.Property(e => e.DeptId).HasColumnName("dept_id").HasComment("部门ID");
        entity.Property(e => e.ParentId).HasColumnName("parent_id").HasComment("上级ID");
        entity.Property(e => e.DeptName).HasColumnName("dept_name").HasComment("部门名称").HasMaxLength(50);
        entity.Property(e => e.SortNo).HasColumnName("sort_no").HasComment("排序号");
        entity.Property(e => e.Disabled).HasColumnName("disabled").HasComment("是否作废");
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注说明").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间");

        entity.HasOne(d => d.Parent).WithMany(p => p.Children)
            .HasForeignKey(d => d.ParentId)
            .HasConstraintName("fk_sys_dept_parent_id");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DeptEntity> modelBuilder);
}
