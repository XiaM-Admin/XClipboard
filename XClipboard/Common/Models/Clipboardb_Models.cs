using System;
using System.ComponentModel;

namespace XClipboard.Common.Models
{
    public class Clipboardb_Models
    {
        /// <summary>
        /// 来自?的APP
        /// </summary>
        public string app { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }

        /// <summary>
        /// 类型（string，image，file）
        /// </summary>
        public string contentType { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime updatatime { get; set; }

        /// <summary>
        /// 用户分类名
        /// </summary>
        public string className { get; set; }
    }
}