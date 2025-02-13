using System.ComponentModel;

namespace FeiNuo.Admin.Models;

public enum MenuTypeEnum
{
    /// <summary>
    /// 模块
    /// </summary>
    [Description("模块")]
    Module = 1001,

    /// <summary>
    /// 菜单
    /// </summary>
    [Description("菜单")]
    Menu = 1002,

    /// <summary>
    /// 按钮
    /// </summary>
    [Description("按钮")]
    Button = 1003,
}
