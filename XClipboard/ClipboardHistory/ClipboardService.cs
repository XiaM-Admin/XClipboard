using DataHandler.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using WK.Libraries.SharpClipboardNS;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;
using XClipboard.Views;

namespace XClipboard.ClipboardHistory
{
    public static class ClipboardService
    {
        private static readonly SharpClipboard clipboard = new();

        public static async Task<bool> InsertImageUri(string path, string Url)
        {
            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];
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
            clipboard.ClipboardChanged += ClipboardChanged;
            clipboard.StartMonitoring();
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            appState.IsClipboardServiceRunning.Clipboard = true;
        }

        public static void Stop()
        {
            clipboard.StopMonitoring();
            clipboard.ClipboardChanged -= ClipboardChanged;
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
            JsonSettings appset = ((AppState)System.Windows.Application.Current.Properties["AppState"]).userSettings.JsonSettings;
            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];

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
                //提示用户上传图片
                NotificationWindow notification = new NotificationWindow(new ViewModels.NotificationWindowModels("提示", "检测到剪贴板图片，是否上传？", button_Ok: async () =>
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

                        //提示用户上传成功
                        NotificationWindow notification_ok = new NotificationWindow(new ViewModels.NotificationWindowModels("提示", "上传成功，是否复制链接至剪贴板中？", button_Ok: () =>
                        {
                            System.Windows.Forms.Clipboard.SetText(Response.ImageUrl);
                        }));
                    }
                    else
                    {
                        //提示用户上传失败
                        NotificationWindow notification_ok = new NotificationWindow(new ViewModels.NotificationWindowModels("提示", "上传失败，请检查网络后重试！"));
                    }
                }));
                notification.Show();
            }
            return id;
        }

        //TODO:模块化ClipboardChanged函数
        private static async void ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            //启动软件后第一次跳过
            if (((AppState)System.Windows.Application.Current.Properties["AppState"]).JumpSave)
            {
                ((AppState)System.Windows.Application.Current.Properties["AppState"]).JumpSave = false;
                return;
            }

            //排除自己 进程名字
            if (e.SourceApplication.Name == Process.GetCurrentProcess().MainModule.FileName || e.SourceApplication.Name == "XClipboard.exe")
                return;

            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];
            string _content = default;
            try
            {
                _content = await handle_Content(e);
            }
            catch (Exception)
            {
                return;
            }

            // 去重调用 保存数据库
            int id = await dbobj.InsertDistinct(new Clipboardb_Models
            {
                app = e.SourceApplication.Name,
                content = _content,
                createtime = DateTime.Now,
                contentType = e.ContentType.ToString()
            }, "content",
            new List<string>()
            {
                "updatatime","className"
            });
            if (id > 0)
                LogManager.GetLogger("ClipboardService").Info($" DataInsert ID {id} Content {e.Content}");
            else
                LogManager.GetLogger("ClipboardService").Error($" DataInsert Error ID {id} Content {e.Content}");

            // 排除内容为空的剪贴板项目
            // TODO: 设置为自定义排除内容
            if (e.ContentType == SharpClipboard.ContentTypes.Text && e.Content.ToString().Trim() is "")
                return;

            //上传图片
            int id_Image = -1;
            if (e.ContentType == SharpClipboard.ContentTypes.Image)
                id_Image = await AutoUpImage(_content);

            if (id_Image > 0)
                LogManager.GetLogger("ClipboardService").Info($"UpLoadImage Insert ID {id} Content {e.Content}");

            if (id > 0 || id_Image > 0)
            {
                //更新缓存数据
                var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
                appState.UpdataClipboards();
            }
        }

        /// <summary>
        /// 处理剪贴板内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<string> handle_Content(SharpClipboard.ClipboardChangedEventArgs e)
        {
            //TODO: 改成自定义选择处理类型
            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];
            string _content = default;
            switch (e.ContentType)
            {
                case SharpClipboard.ContentTypes.Text:
                    _content = e.Content.ToString();
                    break;

                case SharpClipboard.ContentTypes.Image:
                    Image img = (Image)e.Content;
                    //TODO: 设置为设置自定义路径
                    string picname = "Data\\Images\\" + DateTime.Now.ToString("yyyy MM-dd HH-mm-ss") + ".png";
                    if (!Directory.Exists("Data\\Images\\")) Directory.CreateDirectory("Data\\Images\\");

                    //去重图片，调数据库Base64查重
                    string base64 = StorageCommon.ImageToBase64(StorageCommon.CreateThumbnail(img, 100, 100));
                    var list = await dbobj.GetDtaByParam<Imageurldb_Models>("Base64", base64, DBService_Core.ImgaeurlName);
                    if (list is not null)
                        throw new Exception();

                    using (FileStream f = new(picname, FileMode.Create))
                        img.Save(f, ImageFormat.Png);
                    _content = picname;
                    break;

                case SharpClipboard.ContentTypes.Files:
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
                    throw new Exception();

                default:
                    throw new Exception();
            }
            return _content;
        }
    }
}