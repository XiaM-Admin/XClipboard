using System.Collections.Generic;

namespace XClipboard.Common.Models
{
    /// <summary>
    /// 菜单项目
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// 菜单项目的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单项目的图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单项目的命令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 菜单项目的子菜单
        /// </summary>
        public List<MenuItem> Children { get; set; }
    }
}