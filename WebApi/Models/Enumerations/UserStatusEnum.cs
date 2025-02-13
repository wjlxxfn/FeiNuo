using System.ComponentModel;

namespace FeiNuo.Models
{
    public enum UserStatusEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1010,

        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Locked = 1011,

        /// <summary>
        /// 密码过期
        /// </summary>
        [Description("密码过期")]
        Expired = 1012,
    }
}
