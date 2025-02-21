using FeiNuo.Admin.Models;
using Mapster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    public class RoleDtoRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<RoleEntity, RoleDto>().Map(d => d.MenuIds, s => s.Menus.Select(m => m.MenuId));
        }
    }
    #endregion

    #region 数据传输对象 RoleDto
    /// <summary>
    /// 数据传输对象：角色
    /// </summary>
    public class RoleDto : BaseDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Description("角色ID")]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Description("角色编码")]
        [Required(ErrorMessage = "【角色编码】不能为空")]
        [StringLength(50, ErrorMessage = "【角色编码】长度不能超过 50。")]
        public string RoleCode { get; set; } = null!;

        /// <summary>
        /// 角色名称
        /// </summary>
        [Description("角色名称")]
        [Required(ErrorMessage = "【角色名称】不能为空")]
        [StringLength(50, ErrorMessage = "【角色名称】长度不能超过 50。")]
        public string RoleName { get; set; } = null!;

        /// <summary>
        /// 是否作废
        /// </summary>
        [Description("是否作废")]
        public bool Disabled { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Description("备注说明")]
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
		[Description("菜单ID")]
        public IEnumerable<int> MenuIds { get; set; } = Enumerable.Empty<int>();

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：角色
    /// </summary>
    public class RoleQuery : AbstractQuery<RoleEntity>
    {
        /// <summary>
        /// 角色编码精确查找
        /// </summary>
        public string? RoleCode { get; set; }

        /// <summary>
        /// 是否作废
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// 根据查询条件添加查询表达式
        /// </summary>
        protected override void MergeQueryExpression()
        {
            AddExpression(Disabled.HasValue, r => r.Disabled == Disabled!.Value);
            AddExpression(RoleCode, r => r.RoleCode == RoleCode);
            AddSearchExpression(s => o => o.RoleCode.Contains(s) || o.RoleName.Contains(s));
            AddDateExpression(s => o => o.CreateTime >= s, e => o => o.CreateTime <= e);
        }
    }
    #endregion
}
