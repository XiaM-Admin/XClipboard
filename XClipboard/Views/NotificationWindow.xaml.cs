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

            titlt = N.Title;

            // 创建一个计数器变量，用于跟踪已经过去的时间
            int counter = N.ShowTime;
            // 创建一个定时器对象
            var timer = new System.Windows.Forms.Timer();
            // 设置定时器的间隔为1秒
            timer.Interval = 1000;
            // 在定时器的Tick事件中更新窗口的UI元素，并关闭窗口
            timer.Tick += (sender, args) =>
            {
                counter--;
                if (counter == 0)
                {
                    timer.Stop();
                    Close();
                }
                else
                    // 更新窗口的UI元素，以显示剩余的秒数
                    N.Title = titlt + $" ({counter} S)";
            };
            // 启动定时器
            if (N.ShowTime > 0)
            {
                N.Title = titlt + $" ({counter} S)";
                timer.Start();
            }
        }

        private void CloseWindow()
        {
            Close();
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

            //Storyboard sb = new Storyboard();
            //DoubleAnimation da = new DoubleAnimation()
            //{
            //    From = 0,
            //    To = 1,
            //    Duration = new Duration(TimeSpan.FromSeconds(1))
            //};
            //Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            //sb.Children.Add(da);
            //sb.Begin(this);
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

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);


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
        private string titlt;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint LWA_ALPHA = 0x00000002;

        private enum OpacityType
        {
            Transparent = 0,
            Opaque = 255
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}