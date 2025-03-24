using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：角色
/// </summary>
public partial class RoleEntity : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; } = null!;

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = null!;

    /// <summary>
    /// 角色状态：0正常/1作废
    /// </summary>
    public short Status { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    public virtual ICollection<MenuEntity> Menus { get; set; } = [];

    public virtual ICollection<UserEntity> Users { get; set; } = [];
}

/// <summary>
/// 数据库配置：角色
/// </summary>
public partial class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> entity)
    {
        entity.HasKey(e => e.RoleId).HasName("pk_sys_role");

        entity.ToTable("sys_role", tb => tb.HasComment("角色"));

        entity.Property(e => e.RoleId).HasColumnName("role_id").HasComment("角色ID");
        entity.Property(e => e.RoleCode).HasColumnName("role_code").HasComment("角色编码").HasMaxLength(50);
        entity.Property(e => e.RoleName).HasColumnName("role_name").HasComment("角色名称").HasMaxLength(50);
        entity.Property(e => e.Status).HasColumnName("status").HasComment("角色状态：0正常/1作废");
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注说明").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间").HasMaxLength(6);
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间").HasMaxLength(6);

        entity.HasMany(d => d.Menus).WithMany(p => p.Roles)
            .UsingEntity("RoleMenu",
                r => r.HasOne(typeof(MenuEntity)).WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sys_role_menu_menu_id"),
                l => l.HasOne(typeof(RoleEntity)).WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sys_role_menu_role_id"),
                j =>
                {
                    j.HasKey("RoleId", "MenuId").HasName("pk_sys_role_menu");
                    j.ToTable("sys_role_menu", tb => tb.HasComment("角色菜单关联表"));
                    j.IndexerProperty<int>("RoleId").HasComment("角色ID").HasColumnName("role_id");
                    j.IndexerProperty<int>("MenuId").HasComment("菜单ID").HasColumnName("menu_id");
                });

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<RoleEntity> modelBuilder);
}
