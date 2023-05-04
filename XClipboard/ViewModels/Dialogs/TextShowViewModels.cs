using DataHandler.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using XClipboard.ClipboardHistory;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Views;

namespace XClipboard.ViewModels.Dialogs
{
    public class TextShowViewModels : BindableBase, IDialogAware
    {
        /// <summary>
        /// 只读数据库对象
        /// </summary>
        private readonly DBService dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];

        /// <summary>
        /// 显示剪贴板总数据
        /// </summary>
        private ClipboardItems clipboardItem;

        /// <summary>
        /// 是否显示上传图床按钮
        /// </summary>
        private string showButtonView;

        /// <summary>
        /// 是否显示等待进度
        /// </summary>
        private string showDelayTip;

        /// <summary>
        /// 显示图片数据
        /// </summary>
        private BitmapImage showImage;

        /// <summary>
        /// 是否显示下方提示
        /// </summary>
        private bool showTipBool;

        /// <summary>
        /// 下方提示内容
        /// </summary>
        private string showTipText;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dialogService"></param>
        public TextShowViewModels(IDialogService dialogService)
        {
            EditClassName = new DelegateCommand(editClassName);
            UpPicToNet = new DelegateCommand(upPicToNet);
            DialogService = dialogService;
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        public event Action<IDialogResult> RequestClose;
        private int imageWidth = 0;

        public int ImageWidth
        {
            get { return imageWidth; }
            set { imageWidth = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 剪切板总数居
        /// </summary>
        public ClipboardItems ClipboardItem
        {
            get { return clipboardItem; }
            set { clipboardItem = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 对话框接口对象
        /// </summary>
        public IDialogService DialogService { get; }

        /// <summary>
        /// 编辑分类命令
        /// </summary>
        public DelegateCommand EditClassName { private set; get; }

        /// <summary>
        /// 获取的图片数据
        /// </summary>
        public Imageurldb_Models Imageurldb { get; set; }

        /// <summary>
        /// 是否显示上传图片按钮
        /// </summary>
        public string ShowButtonView
        {
            get { return showButtonView; }
            set { showButtonView = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 是否显示等待进度
        /// </summary>
        public string ShowDelayTip
        {
            get { return showDelayTip; }
            set { showDelayTip = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 显示图片数据
        /// </summary>
        public BitmapImage ShowImage
        {
            get { return showImage; }
            set { showImage = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 是否显示下方提示
        /// </summary>
        public bool ShowTipBool
        {
            get { return showTipBool; }
            set { showTipBool = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 下方提示内容
        /// </summary>
        public string ShowTipText
        {
            get { return showTipText; }
            set { showTipText = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title => "更多信息";

        /// <summary>
        /// 上传图片命令
        /// </summary>
        public DelegateCommand UpPicToNet { private set; get; }

        /// <summary>
        /// 能否关闭对话框
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// 对话框接口实现方法
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// 对话框被打开后进入方法
        /// </summary>
        /// <param name="parameters"></param>
        public async void OnDialogOpened(IDialogParameters parameters)
        {
            ClipboardItem = parameters.ContainsKey("content") ? parameters.GetValue<ClipboardItems>("content") : default;

            if (ClipboardItem?.contentType == "Image")
            {
                Imageurldb = await ClipboardItem?.GetContent() as Imageurldb_Models;
                var image = new BitmapImage();
                try
                {
                    if (Imageurldb?.Url is "" or null)
                        throw new Exception();

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(Imageurldb.Url);
                    image.EndInit();
                    ShowImage = image;
                }
                catch (Exception)
                {
                    if (!File.Exists(ClipboardItem.content))
                    {
                        ShowTipText = "图片因为某些原因，文件已不存在，无法显示！";
                        ShowTipBool = true;
                        return;
                    }
                    using MemoryStream stream = new(File.ReadAllBytes(ClipboardItem.content));
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    ShowImage = image;
                    ShowButtonView = "ShowButton";
                }
                ImageWidth = 380;
            }
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        private void editClassName()
        {
            DialogParameters keyValuePairs = new()
            {
                {"Tip","修改分类名称" },
                {"Content",ClipboardItem.className }
            };
            DialogService.ShowDialog("DialogHost_EditView", keyValuePairs, async (t) =>
            {
                //t.Canel = ok 修改数据库分类名 同时修改更新imageItem显示
                if (t.Result == ButtonResult.OK)
                {
                    var str = t.Parameters.ContainsKey("Content") ? t.Parameters.GetValue<string>("Content") : "";
                    if (str.Trim() == "")
                        return;
                    //修改数据库信息
                    Clipboardb_Models obj = await dbobj.ChangeData<Clipboardb_Models>("content", ClipboardItem.content, new
                    {
                        ClassName = str
                    });

                    if (obj is not null)
                        ClipboardItem = new(obj);
                }
            });
        }

        /// <summary>
        /// 上传按钮触发事件
        /// </summary>
        private async void upPicToNet()
        {
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            ShowDelayTip = "Sleep";
            UpLoadResponse response = await Auto_UpImage.UpImageAsync(appState.userSettings.JsonSettings.ImageurlSettings.DefaultOSS, ClipboardItem.content);
            ShowDelayTip = "";
            string sampleMessageDialog;
            if (response.IsSuccessful)
            {
                sampleMessageDialog = "上传完成，已为你复制到剪贴板中";
                System.Windows.Forms.Clipboard.SetText(response.ImageUrl);
                ShowButtonView = "";
                //插入数据库
                await ClipboardService.InsertImageUri(ClipboardItem.content, response.ImageUrl);
            }
            else
                sampleMessageDialog = $"上传失败，{response.Text}";
            ShowTipText = sampleMessageDialog;
            ShowTipBool = true;
            await Task.Delay(1500);
            ShowTipBool = false;
        }
    }
}