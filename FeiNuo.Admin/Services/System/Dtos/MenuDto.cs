using FeiNuo.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    //public class MenuDtoRegister : IRegister
    //{
    //    public void Register(TypeAdapterConfig config)
    //    {
    //        config.ForType<MenuEntity, MenuDto>().Map(d => d.DeptName, s => s.Dept.DeptName, s => s.Dept != null);
    //    }
    //}
    #endregion

    #region 数据传输对象 MenuDto
    /// <summary>
    /// 数据传输对象：菜单
    /// </summary>
    public class MenuDto : BaseDto
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
        [Required(ErrorMessage = "【菜单名称】不能为空")]
        [StringLength(50, ErrorMessage = "【菜单名称】长度不能超过 50。")]
        public string MenuName { get; set; } = null!;

        /// <summary>
        /// 菜单类型
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [Required(ErrorMessage = "【菜单地址】不能为空")]
        [StringLength(200, ErrorMessage = "【菜单地址】长度不能超过 200。")]
        public string MenuPath { get; set; } = null!;

        /// <summary>
        /// 权限标识
        /// </summary>
        [Required(ErrorMessage = "【权限标识】不能为空")]
        [StringLength(200, ErrorMessage = "【权限标识】长度不能超过 200。")]
        public string Permission { get; set; } = null!;

        /// <summary>
        /// 排序号
        /// </summary>
        public short SortNo { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [StringLength(500, ErrorMessage = "【菜单图标】长度不能超过 500。")]
        public string? Icon { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：菜单
    /// </summary>
    public class MenuQuery : AbstractQuery<MenuEntity>
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
