using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：菜单
/// </summary>
public partial class MenuEntity : FeiNuo.Core.BaseEntity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// 上级ID
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = null!;

    /// <summary>
    /// 菜单类型
    /// </summary>
    public int MenuType { get; set; }

    /// <summary>
    /// 菜单地址
    /// </summary>
    public string MenuPath { get; set; } = null!;

    /// <summary>
    /// 权限标识
    /// </summary>
    public string Permission { get; set; } = null!;

    /// <summary>
    /// 排序号
    /// </summary>
    public short SortNo { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    public virtual ICollection<MenuEntity> Children { get; set; } = [];

    public virtual MenuEntity? Parent { get; set; }

    public virtual ICollection<RoleEntity> Roles { get; set; } = [];
}

/// <summary>
/// 数据库配置：菜单
/// </summary>
public partial class MenuConfiguration : IEntityTypeConfiguration<MenuEntity>
{
    public void Configure(EntityTypeBuilder<MenuEntity> entity)
    {
        entity.HasKey(e => e.MenuId).HasName("pk_sys_menu");

        entity.ToTable("sys_menu", tb => tb.HasComment("菜单"));

        entity.HasIndex(e => e.ParentId, "ix_sys_menu_parent_id");

        entity.Property(e => e.MenuId).HasColumnName("menu_id").HasComment("菜单ID");
        entity.Property(e => e.ParentId).HasColumnName("parent_id").HasComment("上级ID");
        entity.Property(e => e.MenuName).HasColumnName("menu_name").HasComment("菜单名称").HasMaxLength(50);
        entity.Property(e => e.MenuType).HasColumnName("menu_type").HasComment("菜单类型");
        entity.Property(e => e.MenuPath).HasColumnName("menu_path").HasComment("菜单地址").HasMaxLength(200).IsUnicode(false);
        entity.Property(e => e.Permission).HasColumnName("permission").HasComment("权限标识").HasDefaultValue("").HasMaxLength(200).IsUnicode(false);
        entity.Property(e => e.SortNo).HasColumnName("sort_no").HasComment("排序号");
        entity.Property(e => e.Icon).HasColumnName("icon").HasComment("菜单图标").HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注说明").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间");

        entity.HasOne(d => d.Parent).WithMany(p => p.Children)
            .HasForeignKey(d => d.ParentId)
            .HasConstraintName("fk_sys_menu_parent_id");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MenuEntity> modelBuilder);
}
