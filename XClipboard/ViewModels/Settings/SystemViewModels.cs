using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using XClipboard.Common;
using XClipboard.Common.Models;

namespace XClipboard.ViewModels.Settings
{
    public class SystemViewModels : BindableBase, INavigationAware
    {
        private JsonSettings _appState;

        private List<string> defaultOSSItems;

        private List<string> show_NumberItems;

        private string showView;

        public SystemViewModels()
        {
            BindAppState = ((AppState)System.Windows.Application.Current.Properties["AppState"]).userSettings.JsonSettings;
            ShowView = "SystemView";
            Show_NumberItems = new List<string> { "5", "6", "7", "8", "9", "10" };
            DefaultOSSItems = new List<string> { "又拍云" };
        }
        public JsonSettings BindAppState
        {
            get { return _appState; }
            set { _appState = value; RaisePropertyChanged(); }
        }

        public List<string> DefaultOSSItems
        {
            get { return defaultOSSItems; }
            set { defaultOSSItems = value; RaisePropertyChanged(); }
        }

        public List<string> Show_NumberItems
        {
            get { return show_NumberItems; }
            set { show_NumberItems = value; RaisePropertyChanged(); }
        }
        public string ShowView
        {
            get { return showView; }
            set { showView = value; RaisePropertyChanged(); }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("View"))
            {
                ShowView = navigationContext.Parameters.GetValue<string>("View");
            }
        }
    }

    /// <summary>
    /// 转换器控制显示布局
    /// </summary>
    public class ViewVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string viewName = parameter as string;
            string showView = value as string;

            if (viewName == showView)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}