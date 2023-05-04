using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;

namespace XClipboard.ViewModels.Dialogs
{
    internal class ImageShowViewModels : BindableBase, IDialogAware
    {
        private readonly DBService dbobj = (DBService)System.Windows.Application.Current.Properties["DBObj"];
        private string copyUrl;
        private ShowImageItem imageItem;
        private BitmapImage showImage;
        private bool showTip;
        private bool IsChanged { get; set; } = false;

        public ImageShowViewModels(IDialogService dialogService)
        {
            ChangeUrl = new DelegateCommand<string>(changurl);
            Copy = new DelegateCommand(copy);
            EditNotes = new DelegateCommand(editnotes);
            EditClassName = new DelegateCommand(editclassname);
            DialogService = dialogService;
        }

        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 修改复制URL格式
        /// </summary>
        public DelegateCommand<string> ChangeUrl { private set; get; }

        /// <summary>
        /// 复制URL
        /// </summary>
        public DelegateCommand Copy { private set; get; }

        /// <summary>
        /// URL
        /// </summary>
        public string CopyUrl
        {
            get { return copyUrl; }
            set { copyUrl = value; RaisePropertyChanged(); }
        }

        public IDialogService DialogService { get; }

        /// <summary>
        /// 编辑类名
        /// </summary>
        public DelegateCommand EditClassName { private set; get; }

        /// <summary>
        /// 编辑备注
        /// </summary>
        public DelegateCommand EditNotes { private set; get; }

        /// <summary>
        /// 图片属性集合
        /// </summary>
        public ShowImageItem ImageItem
        {
            get { return imageItem; }
            set { imageItem = value; RaisePropertyChanged(); }
        }

        /// <summary>
        ///图片资源
        /// </summary>
        public BitmapImage ShowImage
        {
            get { return showImage; }
            set { showImage = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 提示弹出控制flag
        /// </summary>
        public bool ShowTip
        {
            get { return showTip; }
            set { showTip = value; RaisePropertyChanged(); }
        }

        public string Title => "图床 - 图片详情";

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            DialogParameters keyValuePairs = new()
            {
                {"Content",imageItem }
            };
            ButtonResult result = IsChanged ? ButtonResult.OK : ButtonResult.Cancel;
            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(result, keyValuePairs));
        }

        /// <summary>
        /// 载入事件
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
                ImageItem = parameters.GetValue<ShowImageItem>("Value");
            if (ImageItem != default)
            {
                //获取url链接图片数据放到ShowImage中
                var image = new BitmapImage();
                try
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(ImageItem.Url);
                    image.EndInit();
                    ShowImage = image;
                }
                catch (Exception)
                {
                    if (ImageItem.Base64 != default)
                    {
                        // 将Base64字符串转换为byte数组
                        byte[] imageBytes = Convert.FromBase64String(ImageItem.Base64);

                        // 在内存中创建一个流并将byte数组写入该流
                        using MemoryStream stream = new(imageBytes);
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        ShowImage = image;
                    }
                }

                //CopyUrl
                CopyUrl = ImageItem.Url;
            }
        }

        /// <summary>
        /// 改变url格式
        /// </summary>
        /// <param name="obj"></param>
        private void changurl(string obj)
        {
            //BBCode \ MarkDown \ HTML \ URL
            switch (obj)
            {
                case "BBCode":
                    CopyUrl = $"[img]{ImageItem.Url}[/img]";
                    break;

                case "MarkDown":
                    CopyUrl = $"![]({ImageItem.Url})";
                    break;

                case "HTML":
                    CopyUrl = $"<img src=\"{ImageItem.Url}\" alt=\"image\">";
                    break;

                case "URL":
                    CopyUrl = $"{ImageItem.Url}";
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 赋值url
        /// </summary>
        private async void copy()
        {
            // 将选中文本复制到剪贴板
            System.Windows.Forms.Clipboard.SetText(CopyUrl);
            ShowTip = true;
            await Task.Delay(1500);
            ShowTip = false;
        }

        /// <summary>
        /// 编辑类名
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void editclassname()
        {
            DialogParameters keyValuePairs = new()
            {
                {"Tip","修改分类名称" },
                {"Content",ImageItem.ClassName }
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
                    Imageurldb_Models obj = await dbobj.ChangeData<Imageurldb_Models>("Name", ImageItem.Name, new
                    {
                        ClassName = str
                    }, DBService_Core.ImgaeurlName);

                    if (obj is not null)
                    {
                        ImageItem = new(obj);
                        IsChanged = true;
                    }
                }
            });
        }

        /// <summary>
        /// 编辑备注
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void editnotes()
        {
            DialogParameters keyValuePairs = new DialogParameters()
            {
                {"Tip","修改备注信息" },
                {"Content",ImageItem.Notes}
            };
            DialogService.ShowDialog("DialogHost_EditView", keyValuePairs, async (t) =>
            {
                //t.Canel = ok 修改数据库备注信息 同时修改更新imageItem显示
                if (t.Result == ButtonResult.OK)
                {
                    var str = t.Parameters.ContainsKey("Content") ? t.Parameters.GetValue<string>("Content") : "";
                    if (str.Trim() == "")
                        return;
                    //修改数据库信息
                    Imageurldb_Models obj = await dbobj.ChangeData<Imageurldb_Models>("Name", ImageItem.Name, new
                    {
                        Notes = str
                    }, DBService_Core.ImgaeurlName);

                    if (obj is not null)
                    {
                        ImageItem = new(obj);
                        IsChanged = true;
                    }
                }
            });
        }
    }
}