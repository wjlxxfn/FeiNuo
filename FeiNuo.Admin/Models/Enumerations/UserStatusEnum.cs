using System.ComponentModel;

namespace FeiNuo.Admin.Models;

public enum UserStatusEnum
{
    /// <summary>
    /// 正常
    /// </summary>
    [Description("正常")]
    Normal = 0,

    /// <summary>
    /// 锁定
    /// </summary>
    [Description("锁定")]
    Locked = 1,

    /// <summary>
    /// 密码过期
    /// </summary>
    [Description("密码过期")]
    Expired = 2,
}
