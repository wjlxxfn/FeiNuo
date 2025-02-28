using FeiNuo.Admin.Models;
using Mapster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    public class DeptDtoRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DeptEntity, DeptDto>().Map(d => d.ParentName, s => s.Parent == null ? null : s.Parent.DeptName, s => s.Parent != null);
            config.ForType<DeptEntity, DeptDto>().Map(d => d.Children, s => s.Children.OrderBy(t => t.Disabled).ThenBy(a => a.SortNo).Select(a => a.Adapt<DeptDto>()).ToList(), s => s.Children.Count > 0);

            config.ForType<DeptEntity, TreeOption>().ConstructUsing(s => new TreeOption(s.DeptId, s.DeptName, s.Disabled));
            config.ForType<DeptEntity, TreeOption>().Map(d => d.Children, s => s.Children.OrderBy(t => t.Disabled).ThenBy(t => t.SortNo).Select(a => a.Adapt<TreeOption>()).ToList(), s => s.Children.Count > 0);
        }
    }
    #endregion

    #region 数据传输对象 DeptDto
    /// <summary>
    /// 数据传输对象：部门
    /// </summary>
    public class DeptDto : BaseDto
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        [Description("部门ID")]
        public int DeptId { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        [Description("上级ID")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 上级名称
        /// </summary>
        [Description("上级名称")]
        public string? ParentName { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Description("部门名称")]
        [Required(ErrorMessage = "【部门名称】不能为空")]
        [StringLength(50, ErrorMessage = "【部门名称】长度不能超过 50。")]
        public string DeptName { get; set; } = null!;

        /// <summary>
        /// 排序号
        /// </summary>
        [Description("排序号")]
        public short SortNo { get; set; }

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
        /// 下级部门
        /// </summary>
        [Description("下级部门")]
        public List<DeptDto> Children { get; set; } = [];
    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：部门
    /// </summary>
    public class DeptQuery : AbstractQuery<DeptEntity>
    {
        /// <summary>
        /// 上级ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 递归查询，查询当前节点下的所有子节点，当有指定上级节点时才生效果
        /// </summary>
        public bool Recursive { get; set; } = false;

        /// <summary>
        /// 是否作废
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// 根据查询条件添加查询表达式
        /// </summary>
        protected override void MergeQueryExpression()
        {
            if (!Recursive && ParentId.HasValue)
            {
                AddExpression(r => r.ParentId == ParentId.Value);
            }
            AddExpression(Disabled.HasValue, r => r.Disabled == Disabled!.Value);
            AddSearchExpression(s => o => o.DeptName.Contains(s));
            AddDateExpression(s => o => o.CreateTime >= s, e => o => o.CreateTime <= e);
        }
    }
    #endregion
}
