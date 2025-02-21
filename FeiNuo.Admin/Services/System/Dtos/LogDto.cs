using FeiNuo.Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    //public class LogDtoRegister : IRegister
    //{
    //    public void Register(TypeAdapterConfig config)
    //    {
    //        config.ForType<LogEntity, LogDto>().Map(d => d.DeptName, s => s.Dept.DeptName, s => s.Dept != null);
    //    }
    //}
    #endregion

    #region 数据传输对象 LogDto
    /// <summary>
    /// 数据传输对象：操作日志
    /// </summary>
    public class LogDto : BaseDto
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        [Description("日志ID")]
        public long LogId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Description("操作类型")]
        public int OperateType { get; set; }

        /// <summary>
        /// 日志类别(简短描述)
        /// </summary>
        [Description("日志类别(简短描述)")]
        [Required(ErrorMessage = "【日志类别(简短描述)】不能为空")]
        [StringLength(50, ErrorMessage = "【日志类别(简短描述)】长度不能超过 50。")]
        public string LogTitle { get; set; } = null!;

        /// <summary>
        /// 日志内容
        /// </summary>
        [Description("日志内容")]
        [Required(ErrorMessage = "【日志内容】不能为空")]
        public string LogContent { get; set; } = null!;

        /// <summary>
        /// 请求URL
        /// </summary>
        [Description("请求URL")]
        [StringLength(200, ErrorMessage = "【请求URL】长度不能超过 200。")]
        public string? RequestPath { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [Description("请求方式")]
        [StringLength(50, ErrorMessage = "【请求方式】长度不能超过 50。")]
        public string? RequestMethod { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [Description("请求参数")]
        public string? RequestParam { get; set; }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        [Description("是否执行成功")]
        public bool Success { get; set; }

        /// <summary>
        /// 执行时长,毫秒
        /// </summary>
        [Description("执行时长,毫秒")]
        public int ExecuteTime { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Description("客户端IP")]
        [StringLength(50, ErrorMessage = "【客户端IP】长度不能超过 50。")]
        public string? ClientIp { get; set; }

        /// <summary>
        /// 客户端操作系统
        /// </summary>
        [Description("客户端操作系统")]
        [StringLength(50, ErrorMessage = "【客户端操作系统】长度不能超过 50。")]
        public string? ClientOs { get; set; }

        /// <summary>
        /// 客户端浏览器
        /// </summary>
        [Description("客户端浏览器")]
        [StringLength(50, ErrorMessage = "【客户端浏览器】长度不能超过 50。")]
        public string? ClientBrowser { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：操作日志
    /// </summary>
    public class LogQuery : AbstractQuery<LogEntity>
    {
        /// <summary>
        /// 是否作废
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// 根据查询条件添加查询表达式
        /// </summary>
        protected override void MergeQueryExpression()
        {
            // AddExpression(Disabled.HasValue, r => r.Disabled == Disabled!.Value);
            // AddExpression(RoleCode, r => r.RoleCode == RoleCode);
            // AddSearchExpression(s => o => o.RoleCode.Contains(s) || o.RoleName.Contains(s));
            AddDateExpression(s => o => o.CreateTime >= s, e => o => o.CreateTime <= e);
        }
    }
    #endregion
}
