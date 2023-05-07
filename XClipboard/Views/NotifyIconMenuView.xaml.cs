using System;
using System.Windows;
using System.Windows.Input;
using XClipboard.ViewModels;

namespace XClipboard.Views
{
    /// <summary>
    /// NotifyIconMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyIconMenuView : Window
    {
        public NotifyIconMenuView(Action _Show)
        {
            InitializeComponent();
            DataContext = new NotifyIconMenuViewModels(_Show, () =>
            {
                this.Close();
            });
            this.Left = System.Windows.Forms.Cursor.Position.X - this.Width / 2;
            this.Top = System.Windows.Forms.Cursor.Position.Y - this.Height - 10;
            this.Loaded += (sender, e) =>
            {
                // 在窗口加载完成后，自动获取焦点
                this.Activate();
            };
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // 当窗口失去焦点时，关闭窗口
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                //这里直接屏蔽掉异常...
            }
        }

        private bool isDragging = false;
        private Point startPoint;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 当鼠标左键按下时，记录起始点
            isDragging = true;
            startPoint = e.GetPosition(this);
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 当鼠标左键释放时，停止拖动
            isDragging = false;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            // 当鼠标移动时，如果正在拖动，则移动窗口
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                double deltaX = currentPoint.X - startPoint.X;
                double deltaY = currentPoint.Y - startPoint.Y;
                this.Left += deltaX;
                this.Top += deltaY;
            }
        }
    }
}