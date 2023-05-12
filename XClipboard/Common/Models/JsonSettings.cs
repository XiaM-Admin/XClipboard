using System;

namespace XClipboard.Common.Models
{
    /// <summary>
    /// 剪贴板相关设置
    /// </summary>
    public class ClipboardSettings
    {
        /// <summary>
        /// 是否去重保存
        /// </summary>
        public bool duplicate_data { get; set; }

        /// <summary>
        /// 系统图片保存位置
        /// </summary>
        public string image_save_path { get; set; }

        /// <summary>
        /// 需要监控的数据
        /// </summary>
        public Clipboard_HandleData handleData { get; set; }
    }

    /// <summary>
    /// 剪贴板监控数据
    /// </summary>
    public class Clipboard_HandleData
    {
        /// <summary>
        /// 是否监听图片
        /// </summary>
        public bool Image { get; set; }

        /// <summary>
        /// 是否监听文本
        /// </summary>
        public bool Text { get; set; }

        /// <summary>
        /// 是否监听文件路径
        /// </summary>
        public bool Files { get; set; }
    }

    /// <summary>
    /// 图床相关设置
    /// </summary>
    public class ImageurlSettings
    {
        public string CopyUrlMode { get; set; }
        /// <summary>
        /// 默认&选中云存储方式
        /// </summary>
        public string DefaultOSS { get; set; }

        /// <summary>
        /// 又拍云设置
        /// </summary>
        public UpyunSettings UpyunSettings { get; set; }
    }

    /// <summary>
    /// 完整Json设置类
    /// </summary>
    public class JsonSettings
    {
        /// <summary>
        /// 剪贴板相关设置
        /// </summary>
        public ClipboardSettings ClipboardSettings { get; set; }

        /// <summary>
        /// 图库相关设置
        /// </summary>
        public ImageurlSettings ImageurlSettings { get; set; }

        /// <summary>
        /// 系统相关设置
        /// </summary>
        public SystemSettings SystemSettings { get; set; }

        public override bool Equals(object obj)
        {
            //判断类属性是否相同
            if (obj is JsonSettings)
            {
                JsonSettings jsonSettings = obj as JsonSettings;
                if (jsonSettings.ClipboardSettings.duplicate_data == ClipboardSettings.duplicate_data &&
                    jsonSettings.ClipboardSettings.image_save_path == ClipboardSettings.image_save_path &&
                    jsonSettings.ClipboardSettings.handleData.Text == ClipboardSettings.handleData.Text &&
                    jsonSettings.ClipboardSettings.handleData.Image == ClipboardSettings.handleData.Image &&
                    jsonSettings.ClipboardSettings.handleData.Files == ClipboardSettings.handleData.Files &&
                    jsonSettings.ImageurlSettings.CopyUrlMode == ImageurlSettings.CopyUrlMode &&
                    jsonSettings.ImageurlSettings.DefaultOSS == ImageurlSettings.DefaultOSS &&
                    jsonSettings.ImageurlSettings.UpyunSettings.Bucket == ImageurlSettings.UpyunSettings.Bucket &&
                    jsonSettings.ImageurlSettings.UpyunSettings.Host == ImageurlSettings.UpyunSettings.Host &&
                    jsonSettings.ImageurlSettings.UpyunSettings.Password == ImageurlSettings.UpyunSettings.Password &&
                    jsonSettings.ImageurlSettings.UpyunSettings.UpPath == ImageurlSettings.UpyunSettings.UpPath &&
                    jsonSettings.ImageurlSettings.UpyunSettings.User == ImageurlSettings.UpyunSettings.User &&
                    jsonSettings.SystemSettings.Show_Number == SystemSettings.Show_Number &&
                    jsonSettings.SystemSettings.Startup == SystemSettings.Startup &&
                    jsonSettings.SystemSettings.Upload_Alert == SystemSettings.Upload_Alert &&
                    jsonSettings.SystemSettings.Upload_Auto == SystemSettings.Upload_Auto &&
                    jsonSettings.SystemSettings.IsListen == SystemSettings.IsListen)
                    return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

/// <summary>
/// 数据库存储设置模型
/// </summary>
public class JsonSteeingsModels
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 存储Json设置
    /// </summary>
    public string Json { get; set; }
}

/// <summary>
/// 系统相关设置
/// </summary>
public class SystemSettings
{
    /// <summary>
    /// 是否开启剪贴板服务
    /// </summary>
    public bool IsListen { get; set; }

    /// <summary>
    /// 首页展示数据个数
    /// </summary>
    public string Show_Number { get; set; }

    /// <summary>
    /// 开机自启
    /// </summary>
    public bool Startup { get; set; }

    /// <summary>
    /// 上传提醒
    /// </summary>
    public bool Upload_Alert { get; set; }

    /// <summary>
    /// 自动上传
    /// </summary>
    public bool Upload_Auto { get; set; }
}

/// <summary>
/// 又拍云相关设置
/// </summary>
public class UpyunSettings
{
    /// <summary>
    /// 存储桶名
    /// </summary>
    public string Bucket { get; set; }

    /// <summary>
    /// 访问域名
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 管理员密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 上传路径
    /// </summary>
    public string UpPath { get; set; }

    /// <summary>
    /// 管理员账号
    /// </summary>
    public string User { get; set; }
}