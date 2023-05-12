using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;

namespace XClipboard.ViewModels
{
    public class ClipboardItems : Clipboardb_Models
    {
        public ClipboardItems(Clipboardb_Models clipboardb_)
        {
            this.app = clipboardb_.app;
            this.content = clipboardb_.content.TrimStart();
            this.updatatime = clipboardb_.updatatime;
            this.createtime = clipboardb_.createtime;
            this.className = clipboardb_.className;
            this.contentType = clipboardb_.contentType;
        }

        public async Task<object> GetContent()
        {
            if (base.contentType is "Text" or "Files")
            {
                return base.content;
            }
            else if (base.contentType is "Image")
            {
                //获取数据库图片对象返回
                var dbobj = Program_State.GetDBService();
                string name = Path.GetFileName(base.content);
                return await dbobj.GetDtaByParam<Imageurldb_Models>("Name", name, DBService_Core.ImgaeurlName);
            }
            return null;
        }
    }

    public class ClipboardViewModels : BindableBase, INavigationAware
    {
        private readonly DBService dbobj = Program_State.GetDBService();
        private readonly IDialogService dialogService;

        private ObservableCollection<ClipboardItems> clipboards;

        private string comboBox_Text;

        private List<string> comboBoxItems;

        private string contentType;

        private List<string> contentTypeItems;

        private string find_Text;

        private int selectedIndex;

        private object selectedValue;

        private bool showTipBool;

        private string showTipText;

        public ClipboardViewModels(IDialogService dialogService)
        {
            Clipboards = new();
            MouseDoubleClick = new(mousedoubleclick);
            MouseRightButtonDown = new(mouseRightButtonDown);
            Refresh = new(refreshFun);
            Find = new(find);
            ComboBoxItems = new() { "内容", "分类", "来自" };
            ContentTypeItems = new() { "文本", "图片", "文件" };

            this.dialogService = dialogService;
        }

        public ObservableCollection<ClipboardItems> Clipboards
        {
            get { return clipboards; }
            set { clipboards = value; RaisePropertyChanged(); }
        }

        public string ComboBox_Text
        {
            get { return comboBox_Text; }
            set { comboBox_Text = value; RaisePropertyChanged(); }
        }

        public List<string> ComboBoxItems
        {
            get { return comboBoxItems; }
            set { comboBoxItems = value; RaisePropertyChanged(); }
        }

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; RaisePropertyChanged(); }
        }

        public List<string> ContentTypeItems
        {
            get { return contentTypeItems; }
            set { contentTypeItems = value; RaisePropertyChanged(); }
        }

        public DelegateCommand Find { private set; get; }

        public string Find_Text
        {
            get { return find_Text; }
            set { find_Text = value; RaisePropertyChanged(); }
        }

        public DelegateCommand MouseDoubleClick { get; private set; }

        public DelegateCommand MouseRightButtonDown { get; private set; }

        public DelegateCommand Refresh { get; private set; }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }

        public object SelectedValue
        {
            get { return selectedValue; }
            set { selectedValue = value; RaisePropertyChanged(); }
        }

        public bool ShowTipBool
        {
            get { return showTipBool; }
            set { showTipBool = value; RaisePropertyChanged(); }
        }
        public string ShowTipText
        {
            get { return showTipText; }
            set { showTipText = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// INavigationAware接口方法
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// INavigationAware接口方法
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// INavigationAware接口方法
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            reFresh();
        }

        /// <summary>
        /// 查找按钮事件
        /// </summary>
        private void find()
        {
            if (Find_Text is null or "")
            {
                reFresh();
                return;
            }

            if (ComboBox_Text is null or "")
                ComboBox_Text = ComboBoxItems[0];
            if (ContentType is null or "")
                ContentType = ContentTypeItems[0];
            Dictionary<string, string> items_comboBox = new()
            {
                {"所有","ClassName|content|app" },
                {"分类","ClassName" },
                {"内容","content" },
                {"来自","app" }
            };
            Dictionary<string, string> items_contentTypeItems = new()
            {
                {"所有类型","Image|Text|Files" },
                {"图片","Image" },
                {"文本","Text" },
                {"文件","Files" }
            };

            reFresh(items_comboBox[ComboBox_Text], Find_Text, "contentType", items_contentTypeItems[ContentType]);
        }

        /// <summary>
        /// 双击data表事件
        /// </summary>
        private void mousedoubleclick()
        {
            if (SelectedIndex is -1 || SelectedValue is null) return;
            DialogParameters keyValues = new()
            {
                {"content",SelectedValue }
            };
            dialogService.ShowDialog("TextShowView", keyValues, t =>
            {
            });
        }

        /// <summary>
        /// 单击右键表单事件
        /// </summary>
        private async void mouseRightButtonDown()
        {
            if (SelectedIndex is -1 || SelectedValue is null) return;
            ClipboardItems item = SelectedValue as ClipboardItems;
            System.Windows.Forms.Clipboard.SetText(item.content);
            ShowTipText = $"{item.app} 的内容已复制到剪贴板";
            ShowTipBool = true;
            await Task.Delay(1500);
            ShowTipBool = false;
        }

        /// <summary>
        /// 搜索的具体方法
        /// </summary>
        /// <param name="by"></param>
        /// <param name="content"></param>
        /// <param name="by2"></param>
        /// <param name="content2"></param>
        private async void reFresh(string by = "", string content = "", string by2 = "", string content2 = "")
        {
            Clipboards.Clear();
            if (by is not "" && content is not "" && by2 is not "" && content2 is not "")
            {
                List<Clipboardb_Models> list_Find = await dbobj.GetDtaByParams<Clipboardb_Models>(new Dictionary<string, string>
                {
                    {by,content },
                    {by2,content2 }
                }, new
                {
                    app = $"%{content}%",
                    content = $"%{content}%",
                    ClassName = $"%{content}%",
                    contentType = $"%{content2}%"
                });
                list_Find?.Reverse();
                list_Find?.ForEach(t =>
                {
                    Clipboards.Add(new(t));
                });
                return;
            }
            //刷新显示数据
            List<Clipboardb_Models> list = await dbobj.GetAllData<Clipboardb_Models>();
            list?.Reverse();
            list?.ForEach(t =>
            {
                Clipboards.Add(new(t));
            });
        }

        /// <summary>
        /// 刷新事件
        /// </summary>
        private void refreshFun()
        {
            reFresh();
        }
    }
}