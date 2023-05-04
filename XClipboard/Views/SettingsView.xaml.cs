using Prism.Regions;
using System.Windows;
using System.Windows.Controls;
using XClipboard.Core;

namespace XClipboard.Views
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly IRegionManager _regionManager;

        public SettingsView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ///设置默认布局
            _regionManager.RequestNavigate(RegionNames.SettingsRegion, "SystemView");
        }
    }
}