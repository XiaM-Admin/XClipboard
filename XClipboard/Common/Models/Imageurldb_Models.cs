using System;

namespace XClipboard.Common.Models
{
    public class Imageurldb_Models
    {
        /// <summary>
        /// 图片名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 外链地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 用户分类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Base64 { get; set; }

        /// <summary>
        /// 图床上传时间
        /// </summary>
        public DateTime CreateTime { get; set; }


    }
}