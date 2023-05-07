using MaterialDesignThemes.Wpf;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using XClipboard.Core;

namespace XClipboard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRegionManager _regionManager;
        private NotifyIcon _notifyIcon;
        private NotifyIconMenuView _popupWindow;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            // 创建托盘菜单
            // 创建NotifyIcon对象
            _notifyIcon = new();
            try
            {
                _notifyIcon.Icon = new System.Drawing.Icon("Logo.ico"); // 设置托盘图标
            }
            catch (Exception)
            {
                throw new Exception("托盘菜单初始化失败，请检查软件资源是否齐全，若不全请重新安装！");
            }

            _notifyIcon.Visible = true;

            // 添加事件
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.Click += _notifyIcon_Click;

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

        private void _notifyIcon_Click(object sender, EventArgs e)
        {
            //弹出菜单窗口，只显示一个窗口
            _popupWindow?.Close();

            _popupWindow = new NotifyIconMenuView(() =>
            {
                NotifyIcon_DoubleClick(null, null);
            });
            _popupWindow.Show();
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

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // 双击托盘图标时，显示主窗口
            this.Show();
            this.WindowState = WindowState.Normal;
            //置顶后关闭
            this.Topmost = true;
            this.Topmost = false;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // 关闭窗口时，清理托盘图标
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            // 当窗口最小化时，隐藏主窗口并显示托盘图标
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}