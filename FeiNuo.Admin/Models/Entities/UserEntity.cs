using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Admin.Models;

/// <summary>
/// 实体类：用户
/// </summary>
public partial class UserEntity : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 登录用户名
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string Nickname { get; set; } = null!;

    /// <summary>
    /// 登录密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 部门ID
    /// </summary>
    public int DeptId { get; set; }

    /// <summary>
    /// 性别：M/F/O
    /// </summary>
    public string Gender { get; set; } = null!;

    /// <summary>
    /// 手机号码
    /// </summary>
    public string Cellphone { get; set; } = null!;

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 用户状态：0正常/1作废
    /// </summary>
    public byte Status { get; set; }

    /// <summary>
    /// 自我介绍
    /// </summary>
    public string? Introduction { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    public virtual DeptEntity Dept { get; set; } = null!;

    public virtual ICollection<RoleEntity> Roles { get; set; } = [];
}

/// <summary>
/// 数据库配置：用户
/// </summary>
public partial class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        entity.HasKey(e => e.UserId).HasName("pk_sys_user");

        entity.ToTable("sys_user", tb => tb.HasComment("用户"));

        entity.HasIndex(e => e.DeptId, "ix_sys_user_dept_id");

        entity.HasIndex(e => e.Username, "uk_sys_user_username").IsUnique();

        entity.Property(e => e.UserId).HasColumnName("user_id").HasComment("用户ID");
        entity.Property(e => e.Username).HasColumnName("username").HasComment("登录用户名").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.Nickname).HasColumnName("nickname").HasComment("用户昵称").HasMaxLength(50);
        entity.Property(e => e.Password).HasColumnName("password").HasComment("登录密码").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.DeptId).HasColumnName("dept_id").HasComment("部门ID");
        entity.Property(e => e.Gender).HasColumnName("gender").HasComment("性别：M/F/O").IsFixedLength().HasMaxLength(1).IsUnicode(false);
        entity.Property(e => e.Cellphone).HasColumnName("cellphone").HasComment("手机号码").HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.Email).HasColumnName("email").HasComment("电子邮箱").HasMaxLength(100).IsUnicode(false);
        entity.Property(e => e.Avatar).HasColumnName("avatar").HasComment("头像").HasMaxLength(200).IsUnicode(false);
        entity.Property(e => e.Status).HasColumnName("status").HasComment("用户状态：0正常/1作废");
        entity.Property(e => e.Introduction).HasColumnName("introduction").HasComment("自我介绍").HasMaxLength(500);
        entity.Property(e => e.Remark).HasColumnName("remark").HasComment("备注").HasMaxLength(200);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");
        entity.Property(e => e.UpdateBy).HasColumnName("update_by").HasComment("修改人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.UpdateTime).HasColumnName("update_time").HasComment("修改时间");

        entity.HasOne(d => d.Dept).WithMany(p => p.Users)
            .HasForeignKey(d => d.DeptId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_sys_user_dept_id");

        entity.HasMany(d => d.Roles).WithMany(p => p.Users)
            .UsingEntity("UserRole",
                r => r.HasOne(typeof(RoleEntity)).WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sys_user_role_role_id"),
                l => l.HasOne(typeof(UserEntity)).WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sys_user_role_user_id"),
                j =>
                {
                    j.HasKey("UserId", "RoleId").HasName("pk_sys_user_role");
                    j.ToTable("sys_user_role", tb => tb.HasComment("用户角色关联表"));
                    j.IndexerProperty<int>("UserId").HasComment("用户ID").HasColumnName("user_id");
                    j.IndexerProperty<int>("RoleId").HasComment("角色ID").HasColumnName("role_id");
                });

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserEntity> modelBuilder);
}
