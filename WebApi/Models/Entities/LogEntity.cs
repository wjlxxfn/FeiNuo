using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeiNuo.Models;

/// <summary>
/// 实体类：操作日志
/// </summary>
public partial class LogEntity : FeiNuo.Core.BaseEntity
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public long LogId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public int OperateType { get; set; }

    /// <summary>
    /// 日志类别(简短描述)
    /// </summary>
    public string LogTitle { get; set; } = null!;

    /// <summary>
    /// 日志内容
    /// </summary>
    public string LogContent { get; set; } = null!;

    /// <summary>
    /// 请求URL
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? RequestParam { get; set; }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 执行时长,毫秒
    /// </summary>
    public int ExecuteTime { get; set; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 客户端操作系统
    /// </summary>
    public string? ClientOs { get; set; }

    /// <summary>
    /// 客户端浏览器
    /// </summary>
    public string? ClientBrowser { get; set; }
}

/// <summary>
/// 数据库配置：操作日志
/// </summary>
public partial class LogConfiguration : IEntityTypeConfiguration<LogEntity>
{
    public void Configure(EntityTypeBuilder<LogEntity> entity)
    {
        entity.HasKey(e => e.LogId).HasName("pk_sys_log");

        entity.ToTable("sys_log", tb => tb.HasComment("操作日志"));

        entity.HasIndex(e => e.OperateType, "ix_sys_log_operate_type");

        entity.Property(e => e.LogId).HasColumnName("log_id").HasComment("日志ID");
        entity.Property(e => e.OperateType).HasColumnName("operate_type").HasComment("操作类型");
        entity.Property(e => e.LogTitle).HasColumnName("log_title").HasComment("日志类别(简短描述)").HasDefaultValue("").HasMaxLength(50);
        entity.Property(e => e.LogContent).HasColumnName("log_content").HasComment("日志内容").HasDefaultValue("").IsUnicode(false);
        entity.Property(e => e.RequestPath).HasColumnName("request_path").HasComment("请求URL").HasMaxLength(200).IsUnicode(false);
        entity.Property(e => e.RequestMethod).HasColumnName("request_method").HasComment("请求方式").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.RequestParam).HasColumnName("request_param").HasComment("请求参数").IsUnicode(false);
        entity.Property(e => e.Success).HasColumnName("success").HasComment("是否执行成功");
        entity.Property(e => e.ExecuteTime).HasColumnName("execute_time").HasComment("执行时长,毫秒");
        entity.Property(e => e.ClientIp).HasColumnName("client_ip").HasComment("客户端IP").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.ClientOs).HasColumnName("client_os").HasComment("客户端操作系统").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.ClientBrowser).HasColumnName("client_browser").HasComment("客户端浏览器").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateBy).HasColumnName("create_by").HasComment("创建人").HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.CreateTime).HasColumnName("create_time").HasComment("创建时间");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<LogEntity> modelBuilder);
}
