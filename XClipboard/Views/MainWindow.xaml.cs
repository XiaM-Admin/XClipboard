using MaterialDesignThemes.Wpf;
using Prism.Regions;
using System.Windows;
using XClipboard.Core;

namespace XClipboard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRegionManager _regionManager;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            //深浅主题切换
            DarkModeToggleButton.Click += (s, e) =>
            {
                ModifyTheme(DarkModeToggleButton.IsChecked == true);
            };

            CloseWindows.Click += (s, e) => this.Close();
            _regionManager = regionManager;
            //设置窗口左侧抽屉事件
            this.DemoItemsListBox.SelectionChanged += (s, e) => drawrHost.IsLeftDrawerOpen = false;
        }

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new SampleMessageDialog
            {
                Message = { Text = "百度搜索@小夏猪窝 来找我\r\n我的博客 blog.x-tools.top" }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //设置默认布局
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "HomeView");
        }
    }
}