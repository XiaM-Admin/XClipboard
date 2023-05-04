using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;
using XClipboard.Views;

namespace XClipboard.ViewModels
{
    public class SettingsViewModels : BindableBase
    {
        private ObservableCollection<MenuItem> settingsItems;


        public SettingsViewModels(IRegionManager regionManager)
        {
            SettingsItems = new ObservableCollection<MenuItem>();
            CreateMenus();
            RegionManager = regionManager;
            OpenCommand = new DelegateCommand<MenuItem>(Open);
            Save = new DelegateCommand(SaveAsync);

            NavigationParameters keyValuePairs = new()
            {
                {"View","SystemView" }
            };
            //载入默认系统设置视图 RegisterViewWithRegion
            //regionManager.RequestNavigate(RegionNames.SettingsRegion, "SystemView");
        }

        /// <summary>
        /// 导航
        /// </summary>
        public DelegateCommand<MenuItem> OpenCommand { get; private set; }

        public IRegionManager RegionManager { get; }

        public DelegateCommand Save { get; private set; }

        public ObservableCollection<MenuItem> SettingsItems
        {
            get { return settingsItems; }
            set { settingsItems = value; RaisePropertyChanged(); }
        }

        private void CreateMenus()
        {
            SettingsItems.Add(new MenuItem() { Icon = "CogBox", Name = "系统设置", Command = "SystemView" });
            SettingsItems.Add(new MenuItem() { Icon = "ClipboardEditOutline", Name = "剪贴板设置", Command = "ClipboardView" });
            SettingsItems.Add(new MenuItem() { Icon = "ImageOutline", Name = "图库设置", Command = "ImageurlView" });
        }

        private void Open(MenuItem obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Command))
                return;
            NavigationParameters keyValuePairs = new()
            {
                {"View",obj.Command }
            };
            RegionManager.Regions[RegionNames.SettingsRegion].RequestNavigate("SystemView", keyValuePairs);
        }

        private async void SaveAsync()
        {
            SampleMessageDialog sampleMessageDialog;
            var app = (AppState)System.Windows.Application.Current.Properties["AppState"];
            try
            {
                var ret = await app.userSettings.SaveAsync();
                sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "配置已被成功保存！" }
                };
            }
            catch (System.Exception Error)
            {
                sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = $"配置保存失败，原因：\r\n{Error.Message}\r\n请重新设置！" }
                };
            }
            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }
    }
}