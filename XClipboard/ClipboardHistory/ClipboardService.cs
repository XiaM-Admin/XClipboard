using DataHandler.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WK.Libraries.SharpClipboardNS;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;
using XClipboard.ViewModels;
using XClipboard.Views;

namespace XClipboard.ClipboardHistory
{
    public static class ClipboardService
    {
        private static readonly SharpClipboard clipboard = new();

        public static void init()
        {
            LogManager.GetLogger("ClipboardService").Info("ClipboardService Init");
            clipboard.ClipboardChanged += ClipboardChanged;
        }

        public static async Task<bool> InsertImageUri(string path, string Url)
        {
            var dbobj = Program_State.GetDBService();
            //上传成功 记录数据库
            var id = await dbobj.InsertDistinct(new Imageurldb_Models()
            {
                Name = Path.GetFileName(path),
                Url = Url,
                Size = StorageCommon.GetFileSizeString(path),
                CreateTime = DateTime.Now,
                Base64 = StorageCommon.ImageToBase64(StorageCommon.CreateThumbnail(Image.FromFile(path), 100, 100))
            }, "Name",
            new List<string>()
            {
               "ClassName","Notes"
            }, DBService_Core.ImgaeurlName);
            return id > 0;
        }

        public static void Start()
        {
            LogManager.GetLogger("ClipboardService").Info("ClipboardService Start");
            clipboard.StartMonitoring();
            Program_State.GetAppState().IsClipboardServiceRunning.Clipboard = true;
        }

        /// <summary>
        /// 这玩意关不掉... 做个 假停止啊
        /// </summary>
        public static void Stop()
        {
            LogManager.GetLogger("ClipboardService").Info("ClipboardService Stop");
            clipboard.StopMonitoring();
            Program_State.GetAppState().IsClipboardServiceRunning.Clipboard = false;
        }

        /// <summary>
        /// 分析文本 返回合适的类
        /// </summary>
        /// <returns></returns>
        private static string AnalysisText(string txt)
        {
            //分析txt是否为链接格式 用正则表达式
            string pattern = @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            Regex regex = new(pattern);
            Match match = regex.Match(txt);
            if (match.Success)
                return "链接";

            //分析txt是否为邮箱格式 用正则表达式
            pattern = @"^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+";
            regex = new(pattern);
            match = regex.Match(txt);
            if (match.Success)
                return "邮箱";

            return "";
        }

        /// <summary>
        /// 自动上传方法
        /// </summary>
        /// <param name="appset"></param>
        /// <param name="contentTypes"></param>
        /// <param name="_content"></param>
        /// <returns></returns>
        private static async Task<int> AutoUpImage(string _content)
        {
            int id = -1;
            JsonSettings appset = Program_State.GetJsonSettings();
            var dbobj = Program_State.GetDBService();

            if (appset.SystemSettings.Upload_Auto)
            {
                //自动上传图片
                LogManager.GetLogger("ClipboardService").Info($" Start Auto UpLoad This Imge.");
                var Response = await Auto_UpImage.UpImageAsync(appset.ImageurlSettings.DefaultOSS, _content);
                if (Response.IsSuccessful)
                {
                    LogManager.GetLogger("ClipboardService").Info($" Start Auto UpLoad Is OK. Start Add Sqlite.");
                    //上传成功 记录数据库
                    id = await dbobj.InsertDistinct(new Imageurldb_Models()
                    {
                        Name = Path.GetFileName(_content),
                        Url = Response.ImageUrl,
                        Size = StorageCommon.GetFileSizeString(_content),
                        CreateTime = DateTime.Now,
                        Base64 = StorageCommon.ImageToBase64(StorageCommon.CreateThumbnail(Image.FromFile(_content), 100, 100))
                    }, "Name",
                    new List<string>()
                    {
                        "ClassName","Notes"
                    }, DBService_Core.ImgaeurlName);
                    LogManager.GetLogger("ClipboardService").Info($"Add Sqlite OK. Id:{id}");
                }
                else
                {
                    LogManager.GetLogger("ClipboardService").Error($"Auto UpLoadImage Is Error.");
                    LogManager.GetLogger("ClipboardService").Error($"Code: {Response.Code}\r\n" +
                        $"Text:{Response.Text}\r\n" +
                        $"Title:{Response.Title}");
                }
            }
            else if (appset.SystemSettings.Upload_Alert)
            {
                NotificationSets sets = new()
                {
                    Title = "提示",
                    Content = "检测到剪贴板图片，是否上传？",
                    Button_Ok = async () =>
                {
                    //上传
                    LogManager.GetLogger("ClipboardService").Info($" Start UpLoadImage(User).");
                    var Response = await Auto_UpImage.UpImageAsync(appset.ImageurlSettings.DefaultOSS, _content);
                    if (Response.IsSuccessful)
                    {
                        LogManager.GetLogger("ClipboardService").Info($" Start Auto UpLoad Is OK. Start Add Sqlite.");
                        //上传成功 记录数据库
                        id = await dbobj.InsertDistinct(new Imageurldb_Models()
                        {
                            Name = Path.GetFileName(_content),
                            Url = Response.ImageUrl,
                            Size = StorageCommon.GetFileSizeString(_content),
                            CreateTime = DateTime.Now,
                            Base64 = StorageCommon.ImageToBase64(StorageCommon.CreateThumbnail(Image.FromFile(_content), 100, 100))
                        }, "Name",
                        new List<string>() { "ClassName", "Notes" }, DBService_Core.ImgaeurlName);
                        LogManager.GetLogger("ClipboardService").Info($"Add Sqlite OK. Id:{id}");

                        NotificationSets copy = new()
                        {
                            Title = "提示",
                            Content = "上传成功，是否复制链接至剪贴板中？",
                            Button_Ok = () =>
                            {
                                System.Windows.Forms.Clipboard.SetText(Response.ImageUrl);
                            },
                            ShowTime = 5
                        };
                        //提示用户上传成功
                        NotificationWindow notification_ok = new NotificationWindow(new ViewModels.NotificationWindowModels(copy));
                        notification_ok.Show();
                    }
                    else
                    {
                        NotificationSets error = new()
                        {
                            Title = "提示",
                            Content = "上传失败，请检查网络后重试！",
                            ShowTime = 5
                        };
                        //提示用户上传失败
                        NotificationWindow notification_ok = new NotificationWindow(new ViewModels.NotificationWindowModels(error));
                        notification_ok.Show();
                    }
                },
                    ShowTime = 5
                };
                //提示用户上传图片
                NotificationWindow notification = new NotificationWindow(new ViewModels.NotificationWindowModels(sets));
                notification.Show();
            }
            return id;
        }

        private static async void ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            // 监听关闭时跳过
            if (!Program_State.GetJsonSettings().SystemSettings.IsListen)
                return;

            // 启动软件后第一次跳过
            if (Program_State.GetAppState().JumpSave)
            {
                Program_State.GetAppState().JumpSave = false;
                return;
            }

            // 排除自己 进程名字
            if (e.SourceApplication.Name == Process.GetCurrentProcess().MainModule.FileName || e.SourceApplication.Name == "XClipboard.exe")
                return;

            var dbobj = Program_State.GetDBService();
            string _content = default;
            try
            {
                _content = await Handle_Content(e);
            }
            catch (Exception error)
            {
                if (error.Message is not null)
                {
                    NotificationSets showerror = new()
                    {
                        Title = "错误",
                        Content = $"在处理剪贴板数据时发生错误\r\n{error.Message}",
                        ShowTime = 5
                    };
                    //提示用户上传成功
                    NotificationWindow notification_ok = new(new(showerror));
                    notification_ok.Show();
                }
                return;
            }
            string classname = AnalysisText(_content);

            // 去重调用 保存数据库
            int id = await dbobj.InsertDistinct(new Clipboardb_Models
            {
                app = e.SourceApplication.Name,
                content = _content,
                createtime = DateTime.Now,
                contentType = e.ContentType.ToString(),
                className = classname
            }, "content",
            new List<string>()
            {
                "updatatime"
            });
            if (id > 0)
                LogManager.GetLogger("ClipboardService").Info($" DataInsert ID {id} Content {e.Content}");
            else
                LogManager.GetLogger("ClipboardService").Error($" DataInsert Error ID {id} Content {e.Content}");

            // 排除内容为空的剪贴板项目
            if (e.ContentType == SharpClipboard.ContentTypes.Text && e.Content.ToString().Trim() is "")
                return;

            //上传图片
            int id_Image = -1;
            if (e.ContentType == SharpClipboard.ContentTypes.Image)
                id_Image = await AutoUpImage(_content);

            if (id_Image > 0)
                LogManager.GetLogger("ClipboardService").Info($"UpLoadImage Insert ID {id} Content {e.Content}");

            if (id > 0 || id_Image > 0)
                Program_State.GetAppState().UpdataClipboards(); //更新缓存数据
        }

        /// <summary>
        /// 处理剪贴板内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<string> Handle_Content(SharpClipboard.ClipboardChangedEventArgs e)
        {
            JsonSettings appset = Program_State.GetJsonSettings();
            var dbobj = Program_State.GetDBService();

            string _content = default;
            switch (e.ContentType)
            {
                case SharpClipboard.ContentTypes.Text:
                    if (!appset.ClipboardSettings.handleData.Text)
                        throw new Exception(null); // 跳过设置中排除选项

                    _content = e.Content.ToString();
                    break;

                case SharpClipboard.ContentTypes.Image:
                    if (!appset.ClipboardSettings.handleData.Image)
                        throw new Exception(null); // 跳过设置中排除选项
                    Image img = (Image)e.Content;

                    string picname = "";
                    if (appset.ClipboardSettings.image_save_path is not "")
                    {
                        string savepath = appset.ClipboardSettings.image_save_path;
                        //使用自定义路径拼接图片名
                        picname = savepath + "\\" + DateTime.Now.ToString("yyyy MM-dd HH-mm-ss") + ".png";
                        //排除多余的\\
                        picname = picname.Replace("\\\\", "\\");
                        //排除第一个字符\\，如果有
                        if (picname.StartsWith("\\"))
                            picname = picname.Remove(0, 1);
                    }
                    else
                        picname = "Data\\Images\\" + DateTime.Now.ToString("yyyy MM-dd HH-mm-ss") + ".png";
                    try
                    {
                        if (appset.ClipboardSettings.image_save_path == "" && !Directory.Exists("Data\\Images\\")) Directory.CreateDirectory("Data\\Images\\");
                        else if (!Directory.Exists(appset.ClipboardSettings.image_save_path)) Directory.CreateDirectory(appset.ClipboardSettings.image_save_path);
                    }
                    catch (Exception error)
                    {
                        throw new Exception(error.Message);
                    }

                    //去重图片，调数据库Base64查重
                    string base64 = StorageCommon.ImageToBase64(StorageCommon.CreateThumbnail(img, 100, 100));
                    var list = await dbobj.GetDtaByParam<Imageurldb_Models>("Base64", base64, DBService_Core.ImgaeurlName);
                    if (list is not null)
                        throw new Exception(null);

                    using (FileStream f = new(picname, FileMode.Create))
                        img.Save(f, ImageFormat.Png);
                    _content = picname;
                    break;

                case SharpClipboard.ContentTypes.Files:
                    if (!appset.ClipboardSettings.handleData.Files)
                        throw new Exception(null); // 跳过设置中排除选项

                    string txt = "";
                    if (clipboard.ClipboardFiles.Count > 0)
                        foreach (var item in clipboard.ClipboardFiles)
                        {
                            FileInfo fileInfo = new(item);
                            txt += item + "\r\n";
                        }
                    _content = txt;
                    break;

                case SharpClipboard.ContentTypes.Other:
                    _content = e.Content.ToString();
                    break;

                default:
                    throw new Exception(null);
            }
            return _content;
        }
    }
}