using FeiNuo.Admin.Models;
using Mapster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    public class DictDtoRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DictEntity, DictDto>().Map(d => d.DictItems, s => s.DictItems.Adapt<List<DictItemDto>>(), s => s.DictItems != null);
            config.ForType<DictDto, DictEntity>().Ignore(a => a.DictItems);
        }
    }
    #endregion

    #region 数据传输对象 DictDto
    /// <summary>
    /// 数据传输对象：数据字典
    /// </summary>
    public class DictDto : BaseDto
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [Description("字典主键")]
        public int DictId { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [Description("字典类型")]
        [Required(ErrorMessage = "【字典类型】不能为空")]
        [StringLength(50, ErrorMessage = "【字典类型】长度不能超过 50。")]
        public string DictType { get; set; } = null!;

        /// <summary>
        /// 字典名称
        /// </summary>
        [Description("字典名称")]
        [Required(ErrorMessage = "【字典名称】不能为空")]
        [StringLength(50, ErrorMessage = "【字典名称】长度不能超过 50。")]
        public string DictName { get; set; } = null!;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Description("备注说明")]
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

        /// <summary>
        /// 字典项
        /// </summary>
        public List<DictItemDto> DictItems { get; set; } = [];

    }
    #endregion

    #region 数据传输对象 DictItemDto
    /// <summary>
    /// 数据传输对象：字典项
    /// </summary>
    public class DictItemDto : BaseDto
    {
        /// <summary>
        /// 字典项主键
        /// </summary>
        [Description("字典项主键")]
        public int DictItemId { get; set; }

        /// <summary>
        /// 字典主键
        /// </summary>
        [Description("字典主键")]
        public int DictId { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [Description("字典标签")]
        [Required(ErrorMessage = "【字典标签】不能为空")]
        [StringLength(50, ErrorMessage = "【字典标签】长度不能超过 50。")]
        public string DictLabel { get; set; } = null!;

        /// <summary>
        /// 字典键值
        /// </summary>
        [Description("字典键值")]
        [Required(ErrorMessage = "【字典键值】不能为空")]
        [StringLength(200, ErrorMessage = "【字典键值】长度不能超过 200。")]
        public string DictValue { get; set; } = null!;

        /// <summary>
        /// 其他配置
        /// </summary>
        [Description("其他配置")]
        [StringLength(500, ErrorMessage = "【其他配置】长度不能超过 500。")]
        public string? ExtValue { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        [Description("排序序号")]
        public short SortNo { get; set; }

        /// <summary>
        /// 字典状态
        /// </summary>
        [Description("字典状态")]
        public bool Disabled { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Description("备注说明")]
        [StringLength(200, ErrorMessage = "【备注说明】长度不能超过 200。")]
        public string? Remark { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：数据字典
    /// </summary>
    public class DictQuery : AbstractQuery<DictEntity>
    {
        /// <summary>
        /// 字典类型
        /// </summary>
        public string? DictType { get; set; }

        /// <summary>
        /// 根据查询条件添加查询表达式
        /// </summary>
        protected override void MergeQueryExpression()
        {
            AddExpression(!string.IsNullOrEmpty(DictType), r => r.DictType == DictType);
            AddSearchExpression(s => o => o.DictType.Contains(s) || o.DictName.Contains(s));
            //AddDateExpression(s => o => o.CreateTime >= s, e => o => o.CreateTime <= e);
        }
    }
    #endregion
}
