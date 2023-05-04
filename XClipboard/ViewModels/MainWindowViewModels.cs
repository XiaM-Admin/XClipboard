using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;

namespace XClipboard.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// 菜单item
        /// </summary>
        private ObservableCollection<MenuItem> menuItems;

        /// <summary>
        /// 构造函数 new、创建菜单ui_item
        /// </summary>
        /// <param name="regionManager"></param>
        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            menuItems = new ObservableCollection<MenuItem>();
            OpenCommand = new DelegateCommand<MenuItem>(Open);
            RegionManager = regionManager;
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            appState.dialogService = dialogService;
            CreateMenus();
        }

        /// <summary>
        /// 菜单项目列表
        /// </summary>
        public ObservableCollection<MenuItem> MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 导航
        /// </summary>
        public DelegateCommand<MenuItem> OpenCommand { get; private set; }

        /// <summary>
        /// 公用区域切换视图接口对象
        /// </summary>
        public IRegionManager RegionManager { get; }
        /// <summary>
        /// 创建菜单项目
        /// </summary>
        private void CreateMenus()
        {
            menuItems.Add(new MenuItem() { Name = "首页", Icon = "HomeOutline", Command = "HomeView" });
            menuItems.Add(new MenuItem() { Name = "历史", Icon = "Contain", Command = "ClipboardView" });
            //menuItems.Add(new MenuItem() { Name = "截图", Icon = "ClipboardMultipleOutline" });
            menuItems.Add(new MenuItem() { Name = "图库", Icon = "PanoramaVariantOutline", Command = "ImgesUrlView" });
            menuItems.Add(new MenuItem() { Name = "设置", Icon = "CogOutline", Command = "SettingsView" });
        }
        /// <summary>
        /// 切换至某菜单
        /// </summary>
        /// <param name="obj"></param>
        private void Open(MenuItem obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Command))
                return;
            NavigationParameters keyValues = new()
            {
            };
            if (obj.Name is "图库")
            {
                keyValues.Add("UpdataView", "true");
            }
            RegionManager.Regions[RegionNames.ContentRegion].RequestNavigate(obj.Command, keyValues);
        }
    }
}