using MaterialDesignThemes.Wpf;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Forms;
using XClipboard.Common;
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

            // 设置托盘图标
            _notifyIcon.Icon = new System.Drawing.Icon(System.Reflection.Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("XClipboard.Images.Logo.ico"));

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

            // 如果为开机自启动 设置为默认后台运行
            if (Program_State.GetAppState().IsAutoStart)
                this.Hide();
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


        private void Window_StateChanged(object sender, EventArgs e)
        {
            // 当窗口最小化时，隐藏主窗口并显示托盘图标
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                // 弹出提示是否关闭
                if (System.Windows.MessageBox.Show("是否关闭程序？\r\n取消后程序将后台运行.", "真的要走吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    // 关闭窗口时，清理托盘图标
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                    // 关闭程序
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                    this.Hide();
                    // 取消关闭操作
                    e.Cancel = true;
                }
            }
        }
    }
}