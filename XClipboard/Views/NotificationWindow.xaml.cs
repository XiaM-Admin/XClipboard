using Prism.Services.Dialogs;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using XClipboard.ViewModels;

namespace XClipboard.Views
{
    /// <summary>
    /// NotificationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow(NotificationWindowModels N)
        {
            InitializeComponent();
            N.CloseWindow = CloseWindow;
            DataContext = N;
        }
        private void CloseWindow()
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // 获取窗口句柄
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            // 设置窗口为无边框窗口
            SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_SYSMENU | WS_MINIMIZEBOX);
            // 设置窗口为顶层窗口
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            // 设置窗口不接受焦点
            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE);
            // 设置窗口透明
            SetLayeredWindowAttributes(hwnd, 0, (byte)OpacityType.Transparent, LWA_ALPHA);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 计算桌面右下角的位置
            Rect workArea = SystemParameters.WorkArea;
            double x = workArea.Right - ActualWidth - 10;
            double y = workArea.Bottom - ActualHeight - 10;
            Left = x;
            Top = y;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 拖动窗口
            DragMove();
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 鼠标进入窗口时，设置窗口不透明
            SetLayeredWindowAttributes(new WindowInteropHelper(this).Handle, 0, (byte)OpacityType.Opaque, LWA_ALPHA);
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 鼠标离开窗口时，设置窗口透明
            SetLayeredWindowAttributes(new WindowInteropHelper(this).Handle, 0, (byte)OpacityType.Transparent, LWA_ALPHA);
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const uint WS_POPUP = 0x80000000;
        private const uint WS_SYSMENU = 0x00080000;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_EX_TOOLWINDOW = 0x00000080;
        private const uint WS_EX_TRANSPARENT = 0x00000020;
        private const uint WS_EX_NOACTIVATE = 0x08000000;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint LWA_ALPHA = 0x00000002;
        private enum OpacityType
        {
            Transparent = 0,
            Opaque = 255
        }
    }
}