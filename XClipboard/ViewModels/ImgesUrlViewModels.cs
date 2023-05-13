using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;

namespace XClipboard.ViewModels
{
    public class ImgesUrlViewModels : BindableBase, INavigationAware
    {
        /// <summary>
        /// 作用域：类 数据库操作对象
        /// </summary>
        private readonly DBService dbobj = (DBService)Application.Current.Properties["DBObj"];

        private string comboBox_Text;

        private List<string> comboBoxItems;

        private string find_Text;

        /// <summary>
        /// ui展示数据集合
        /// </summary>
        private ObservableCollection<ShowImageItem> imageUrls;

        /// <summary>
        /// 控制提示Flag
        /// </summary>
        private bool showTip;

        /// <summary>
        /// 构造函数
        /// new、更新ui 操作
        /// </summary>
        /// <param name="dialogService"></param>
        public ImgesUrlViewModels(IDialogService dialogService)
        {
            ImageUrls = new ObservableCollection<ShowImageItem>();
            Refresh = new DelegateCommand(refresh);
            Find = new DelegateCommand(find);
            ComboBoxItems = new() { "所有", "分类", "名字", "备注" };
            SelectionChanged = new DelegateCommand(selectionChanged);
            DialogService = dialogService;
        }
        //============================
        private List<string> classItems;

        public List<string> ClassItems
        {
            get { return classItems; }
            set { classItems = value; RaisePropertyChanged(); }
        }

        private string classSelectedItem;

        public string ClassSelectedItem
        {
            get { return classSelectedItem; }
            set { classSelectedItem = value; RaisePropertyChanged(); }
        }

        private string classSelectedValue;

        public string ClassSelectedValue
        {
            get { return classSelectedValue; }
            set { classSelectedValue = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SelectionChanged { get; private set; }

        private void selectionChanged()
        {
            UpdataView("className", ClassSelectedItem);
        }

        private async void AddClassItems()
        {
            var db = Program_State.GetDBService();
            List<string> strings = await db.GetDistinctList("className", DBService_Core.ImgaeurlName);
            ClassItems = new();
            strings.ForEach(t =>
            {
                if (t != string.Empty)
                    ClassItems.Add(t);
            });
        }

        //==========================

        /// <summary>
        /// 多选框文本
        /// </summary>
        public string ComboBox_Text
        {
            get { return comboBox_Text; }
            set { comboBox_Text = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 多选框集合
        /// </summary>
        public List<string> ComboBoxItems
        {
            get { return comboBoxItems; }
            set { comboBoxItems = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 更多设置窗口打开接口
        /// </summary>
        public IDialogService DialogService { get; }

        /// <summary>
        /// 查询图库数据
        /// </summary>
        public DelegateCommand Find { private set; get; }

        /// <summary>
        /// 搜索框文本
        /// </summary>
        public string Find_Text
        {
            get { return find_Text; }
            set { find_Text = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 图片类型集合
        /// </summary>
        public ObservableCollection<ShowImageItem> ImageUrls
        {
            get { return imageUrls; }
            set { imageUrls = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 刷新命令
        /// </summary>
        public DelegateCommand Refresh { private set; get; }

        /// <summary>
        /// 控制提示Flag
        /// </summary>
        public bool ShowTip
        {
            get { return showTip; }
            set { showTip = value; RaisePropertyChanged(); }
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
        { }

        /// <summary>
        /// INavigationAware接口方法
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UpdataView();
            AddClassItems();
        }

        /// <summary>
        /// 复制链接至剪贴板
        /// </summary>
        /// <param name="obj"></param>
        private async void copy_fun(ShowImageItem obj)
        {
            string copymode = Program_State.GetJsonSettings().ImageurlSettings.CopyUrlMode;

            string url = obj.Url.Replace(" ", "%20");
            if (copymode is "" or "Url")
                System.Windows.Forms.Clipboard.SetText(url);
            else if (copymode is "MarkDown")
                System.Windows.Forms.Clipboard.SetText($"![{obj.Name}]({url})");
            else if (copymode is "BBCode")
                System.Windows.Forms.Clipboard.SetText($"[img]{url}[/img]");
            else if (copymode is "HTML")
                System.Windows.Forms.Clipboard.SetText($"<img src=\"{url}\" alt=\"{obj.Name}\">");

            ShowTip = true;
            await Task.Delay(1500);
            ShowTip = false;
        }

        /// <summary>
        /// 查询图库数据方法
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void find()
        {
            if (Find_Text is null or "")
            {
                UpdataView();
                return;
            }

            if (ComboBox_Text is null or "")
                ComboBox_Text = ComboBoxItems[0];
            Dictionary<string, string> keyValues = new()
            {
                {"所有","ClassName|Name|Notes" },
                {"分类","ClassName" },
                {"名字","Name" },
                {"备注","Notes" },
            };
            UpdataView(keyValues[ComboBox_Text], Find_Text);
        }

        /// <summary>
        /// 更多设置
        /// </summary>
        /// <param name="obj"></param>
        private void moveset(ShowImageItem obj)
        {
            ShowImageItem ret = obj;
            DialogParameters keyValuePairs = new()
            {
                {"Value", ret}
            };
            DialogService.ShowDialog("ImageShowView", keyValuePairs, async t =>
            {
                Imageurldb_Models ret1 = await dbobj.GetDtaByParam<Imageurldb_Models>("Name", ret.Name, DBService_Core.ImgaeurlName);
                ShowImageItem showImage = new(ret1);
                if (!(showImage.Equals(obj)))
                    UpdataView();//更新视图
            });
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void refresh()
        {
            UpdataView();
            AddClassItems();
            ClassSelectedValue = "Any";
        }

        /// <summary>
        /// 更新视图显示
        /// </summary>
        private async void UpdataView(string by = null, string find = null)
        {
            ImageUrls.Clear();
            if (find is not null && by is not null)
            {
                List<Imageurldb_Models> db = await dbobj.GetDtaByParams<Imageurldb_Models>(by, find, DBService_Core.ImgaeurlName);
                db.ForEach(x =>
                {
                    // 创建一个新的BitmapImage对象
                    BitmapImage bitmap = new();
                    if (x.Base64 != null && x.Base64 != string.Empty)
                    {
                        // 将Base64字符串转换为byte数组
                        byte[] imageBytes = Convert.FromBase64String(x.Base64);

                        // 在内存中创建一个流并将byte数组写入该流
                        using MemoryStream stream = new(imageBytes);
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }

                    // 创建一个用于显示提示消息的变量 ClassName -> Notes -> Name -> CreateTime
                    string bottomstr = x.Name;
                    if (x.Notes != null && x.Notes != string.Empty)
                        bottomstr = x.Notes;
                    if (x.ClassName != null && x.ClassName != string.Empty)
                        bottomstr = x.ClassName;
                    ImageUrls.Add(new ShowImageItem(x)
                    {
                        ShowImage = bitmap,
                        ShowBottom = bottomstr,
                        OpenSet = new DelegateCommand<ShowImageItem>(moveset),
                        Copy_Fun = new DelegateCommand<ShowImageItem>(copy_fun)
                    });
                });
            }
            else
                //取所有图片数据 GetAllData
                dbobj.GetAllData<Imageurldb_Models>(DBService_Core.ImgaeurlName).Result.ForEach(x =>
                {
                    // 创建一个新的BitmapImage对象
                    BitmapImage bitmap = new();
                    if (x.Base64 != null && x.Base64 != string.Empty)
                    {
                        // 将Base64字符串转换为byte数组
                        byte[] imageBytes = Convert.FromBase64String(x.Base64);

                        // 在内存中创建一个流并将byte数组写入该流
                        using MemoryStream stream = new(imageBytes);
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }

                    // 创建一个用于显示提示消息的变量 ClassName -> Notes -> Name -> CreateTime
                    string bottomstr = x.Name;
                    if (x.Notes != null && x.Notes != string.Empty)
                        bottomstr = x.Notes;
                    if (x.ClassName != null && x.ClassName != string.Empty)
                        bottomstr = x.ClassName;
                    ImageUrls.Add(new ShowImageItem(x)
                    {
                        ShowImage = bitmap,
                        ShowBottom = bottomstr,
                        OpenSet = new DelegateCommand<ShowImageItem>(moveset),
                        Copy_Fun = new DelegateCommand<ShowImageItem>(copy_fun)
                    });
                });

            //时间倒叙排序
            var sortedItems = ImageUrls.OrderByDescending(item => item.CreateTime).ToList();
            var newShowImageItems = new ObservableCollection<ShowImageItem>(sortedItems);
            ImageUrls.Clear();
            foreach (var item in newShowImageItems)
            {
                ImageUrls.Add(item);
            }
        }
    }

    public class ShowImageItem : Imageurldb_Models
    {
        /// <summary>
        /// 构造ShowImageItem
        /// </summary>
        /// <param name="x"></param>
        public ShowImageItem(Imageurldb_Models x)
        {
            Url = x.Url;
            Name = x.Name;
            Notes = x.Notes;
            Size = x.Size;
            ClassName = x.ClassName;
            Base64 = x.Base64;
            CreateTime = x.CreateTime;
        }

        /// <summary>
        /// 复制链接
        /// </summary>
        public DelegateCommand<ShowImageItem> Copy_Fun { set; get; }

        /// <summary>
        /// 显示更多
        /// </summary>
        public DelegateCommand<ShowImageItem> OpenSet { get; set; }

        /// <summary>
        /// 底部文字显示
        /// </summary>
        public string ShowBottom { get; set; }

        /// <summary>
        /// Base64缩略图显示
        /// </summary>
        public BitmapImage ShowImage { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ShowImageItem)) return false;
            ShowImageItem o = (ShowImageItem)obj;
            if (o.Name == this.Name && o.ClassName == this.ClassName
                && o.Notes == this.Notes && o.Size == this.Size
                && o.Base64 == this.Base64 && o.CreateTime == this.CreateTime)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}