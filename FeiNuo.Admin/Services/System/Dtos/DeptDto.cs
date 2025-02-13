using FeiNuo.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    //public class DeptDtoRegister : IRegister
    //{
    //    public void Register(TypeAdapterConfig config)
    //    {
    //        config.ForType<DeptEntity, DeptDto>().Map(d => d.DeptName, s => s.Dept.DeptName, s => s.Dept != null);
    //    }
    //}
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
        public int DeptId { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required(ErrorMessage = "【部门名称】不能为空")]
        [StringLength(50, ErrorMessage = "【部门名称】长度不能超过 50。")]
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
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：部门
    /// </summary>
    public class DeptQuery : AbstractQuery<DeptEntity>
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
