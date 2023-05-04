using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XClipboard.Common.Models;
using XClipboard.Core;

namespace XClipboard.Common
{
    public class AppState
    {
        private static readonly AppState instance = new AppState();

        private ObservableCollection<ListsModels> clipboardListNumber;
        private ProgramState isClipboardServiceRunning;

        private ObservableCollection<ClipboardbLists> newClipboards;
        private DateTime startTime;

        public UserSettings userSettings { get; set; }

        public bool JumpSave { get; set; } = true;

        public IDialogService dialogService { get; set; }

        public AppState()
        {
            // 初始化状态
            IsClipboardServiceRunning = new ProgramState()
            {
                Color = "Red",
                IsRunTrue = false,
                Clipboard = false,
                LocalStorage = false,
                Text = "正在初始化"
            };
            // 记录当前时间
            StartTime = DateTime.Now;
            // 初始化NewClipboards
            NewClipboards = new ObservableCollection<ClipboardbLists>();
            ClipboardListNumber = new ObservableCollection<ListsModels>()
            {
                new ListsModels() { icon = "AlignHorizontalLeft", text = "数据库数：", number = 0},
                new ListsModels() { icon = "FileImage", text = "数据库图片数：", number = 0},
                new ListsModels() { icon = "LinkVariant", text = "图库外链数：", number = 0},
                new ListsModels() { icon = "ArrowLeft", text = "昨天共记录：", number = 0}
            };
        }

        public static AppState Instance
        {
            get { return instance; }
        }

        public ObservableCollection<ListsModels> ClipboardListNumber
        {
            get { return clipboardListNumber; }
            set { clipboardListNumber = value; }
        }

        /// <summary>
        /// 剪贴板服务状态
        /// </summary>
        public ProgramState IsClipboardServiceRunning
        {
            get { return isClipboardServiceRunning; }
            set { isClipboardServiceRunning = value; }
        }

        /// <summary>
        /// 新剪切板数据
        /// </summary>
        public ObservableCollection<ClipboardbLists> NewClipboards
        {
            get { return newClipboards; }
            set { newClipboards = value; }
        }

        /// <summary>
        /// 软件启动时间
        /// </summary>
        private DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// 共运行时长
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTotalRunningTime()
        {
            return DateTime.Now - StartTime;
        }

        public async void UpdataClipboards()
        {
            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];
            List<Clipboardb_Models> clipboardbs = await dbobj.GetData<Clipboardb_Models>(5);
            if (clipboardbs.Count > 0)
            {
                NewClipboards.Clear();
                foreach (var item in clipboardbs)
                {
                    var li = new ClipboardbLists()
                    {
                        icon = "",
                        command = "",
                        app = item.app,
                        content = item.content,
                        createtime = item.createtime,
                        contentType = item.contentType,
                        updatatime = item.updatatime,
                        className = item.className
                    };
                    li.icon = li.contentType switch
                    {
                        "Text" => "FormatText",
                        "Image" => "ImageArea",
                        "Files" => "FileOutline",
                        _ => "CircleOffOutline",
                    };
                    li.content = li.content.Trim();
                    NewClipboards.Add(li);
                }
            }
            UpdataListNumber();
        }

        private void UpdataListNumber()
        {
            var dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];

            ClipboardListNumber.Clear();
            ClipboardListNumber.Add(new ListsModels() { icon = "AlignHorizontalLeft", text = "数据库数：", number = dbobj.GetDataNumber().Result });
            ClipboardListNumber.Add(new ListsModels() { icon = "FileImage", text = "数据库图片数：", number = dbobj.GetNumberByContent("contentType", "Image").Result });
            ClipboardListNumber.Add(new ListsModels() { icon = "LinkVariant", text = "图库外链数：", number = dbobj.GetDataNumber(DBService_Core.ImgaeurlName).Result });
            ClipboardListNumber.Add(new ListsModels() { icon = "ArrowLeft", text = "昨天共记录：", number = dbobj.GetNumberByCreateTime(DateTime.Now).Result });
        }
    }
}