using FeiNuo.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace FeiNuo.Admin.Services.System
{
    #region DTO属性映射    
    //public class UserDtoRegister : IRegister
    //{
    //    public void Register(TypeAdapterConfig config)
    //    {
    //        config.ForType<UserEntity, UserDto>().Map(d => d.DeptName, s => s.Dept.DeptName, s => s.Dept != null);
    //    }
    //}
    #endregion

    #region 数据传输对象 UserDto
    /// <summary>
    /// 数据传输对象：用户
    /// </summary>
    public class UserDto : BaseDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        [Required(ErrorMessage = "【登录用户名】不能为空")]
        [StringLength(50, ErrorMessage = "【登录用户名】长度不能超过 50。")]
        public string Username { get; set; } = null!;

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Required(ErrorMessage = "【用户昵称】不能为空")]
        [StringLength(50, ErrorMessage = "【用户昵称】长度不能超过 50。")]
        public string Nickname { get; set; } = null!;

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required(ErrorMessage = "【登录密码】不能为空")]
        [StringLength(50, ErrorMessage = "【登录密码】长度不能超过 50。")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 部门ID
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 性别：M/F/O
        /// </summary>
        [Required(ErrorMessage = "【性别：M/F/O】不能为空")]
        [StringLength(1, ErrorMessage = "【性别：M/F/O】长度不能超过 1。")]
        public string Gender { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "【手机号码】不能为空")]
        [StringLength(20, ErrorMessage = "【手机号码】长度不能超过 20。")]
        public string Cellphone { get; set; } = null!;

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [StringLength(100, ErrorMessage = "【电子邮箱】长度不能超过 100。")]
        public string? Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(200, ErrorMessage = "【头像】长度不能超过 200。")]
        public string? Avatar { get; set; }

        /// <summary>
        /// 用户状态:正常，锁定，密码过期等
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 是否作废
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 自我介绍
        /// </summary>
        [StringLength(500, ErrorMessage = "【自我介绍】长度不能超过 500。")]
        public string? Introduction { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200, ErrorMessage = "【备注】长度不能超过 200。")]
        public string? Remark { get; set; }

    }
    #endregion

    #region 数据查询对象
    /// <summary>
    /// 查询模型：用户
    /// </summary>
    public class UserQuery : AbstractQuery<UserEntity>
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
