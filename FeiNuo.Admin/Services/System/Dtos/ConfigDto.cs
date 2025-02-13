using FeiNuo.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    //public class ConfigDtoRegister : IRegister
    //{
    //    public void Register(TypeAdapterConfig config)
    //    {
    //        config.ForType<ConfigEntity, ConfigDto>().Map(d => d.DeptName, s => s.Dept.DeptName, s => s.Dept != null);
    //    }
    //}
    #endregion

    #region 数据传输对象 ConfigDto
    /// <summary>
    /// 数据传输对象：参数配置
    /// </summary>
    public class ConfigDto : BaseDto
    {
        /// <summary>
        /// 参数配置ID
        /// </summary>
        public int ConfigId { get; set; }

        /// <summary>
        /// 参数编码
        /// </summary>
        [Required(ErrorMessage = "【参数编码】不能为空")]
        [StringLength(100, ErrorMessage = "【参数编码】长度不能超过 100。")]
        public string ConfigCode { get; set; } = null!;

        /// <summary>
        /// 参数名称
        /// </summary>
        [Required(ErrorMessage = "【参数名称】不能为空")]
        [StringLength(50, ErrorMessage = "【参数名称】长度不能超过 50。")]
        public string ConfigName { get; set; } = null!;

        /// <summary>
        /// 配置内容
        /// </summary>
        [Required(ErrorMessage = "【配置内容】不能为空")]
        [StringLength(500, ErrorMessage = "【配置内容】长度不能超过 500。")]
        public string ConfigValue { get; set; } = null!;

        /// <summary>
        /// 其他配置
        /// </summary>
        [StringLength(500, ErrorMessage = "【其他配置】长度不能超过 500。")]
        public string? ExtraValue { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：参数配置
    /// </summary>
    public class ConfigQuery : AbstractQuery<ConfigEntity>
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
