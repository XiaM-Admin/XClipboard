using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XClipboard.Common;
using XClipboard.Core;

namespace XClipboard.ViewModels
{
    public class NotifyIconMenuViewModels : BindableBase
    {

        public NotifyIconMenuViewModels(Action show = null, Action hide = null)
        {
            MouseUp = new DelegateCommand(mouseUp);
            MenuItems = new ObservableCollection<NotifyIconMenuItems>();
            AddItems(Program_State.GetAppState().IsClipboardServiceRunning.Clipboard);
            Show = show;
            Hide = hide;
        }
        private object selectedMenuItem;

        public object SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set { selectedMenuItem = value; RaisePropertyChanged(); }
        }


        private void mouseUp()
        {
            if (SelectedMenuItem is null)
                return;
            var item = SelectedMenuItem as NotifyIconMenuItems;
            AppState app = Program_State.GetAppState();
            var set = Program_State.GetJsonSettings();
            switch (item.Text)
            {
                case "   关闭":
                    System.Windows.Application.Current.Shutdown();
                    break;
                case "   首页":
                    app.regionManager.RequestNavigate(RegionNames.ContentRegion, "HomeView");
                    Show.Invoke();
                    break;
                case "   图库":
                    app.regionManager.RequestNavigate(RegionNames.ContentRegion, "ImgesUrlView");
                    Show.Invoke();
                    break;
                case "监控停止":
                    app.IsClipboardServiceRunning.Clipboard = true;
                    set.SystemSettings.IsListen = true;
                    break;
                case "正在监控":
                    app.IsClipboardServiceRunning.Clipboard = false;
                    set.SystemSettings.IsListen = false;
                    break;
                default:
                    break;
            }
            Hide.Invoke();
            AddItems(Program_State.GetAppState().IsClipboardServiceRunning.Clipboard);
        }

        public DelegateCommand MouseUp { private set; get; }


        private void AddItems(bool stat)
        {
            if (stat)
                MenuItems.Add(new NotifyIconMenuItems() { Icon = "CheckBold", Text = "正在监控" });
            else
                MenuItems.Add(new NotifyIconMenuItems() { Icon = "CloseThick", Text = "监控停止" });

            MenuItems.Add(new NotifyIconMenuItems() { Icon = "HomeOutline", Text = "   首页" });
            MenuItems.Add(new NotifyIconMenuItems() { Icon = "ImgesUrlView", Text = "   图库" });
            MenuItems.Add(new NotifyIconMenuItems() { Icon = "CloseCircleOutline", Text = "   关闭" });
        }

        private ObservableCollection<NotifyIconMenuItems> menuItems;

        public ObservableCollection<NotifyIconMenuItems> MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; RaisePropertyChanged(); }
        }

        public Action Show { get; }
        public Action Hide { get; }
    }

    public class NotifyIconMenuItems
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Command { get; set; }
    }

}
