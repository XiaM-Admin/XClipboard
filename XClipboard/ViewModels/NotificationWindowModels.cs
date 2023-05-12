using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XClipboard.ViewModels
{
    public class NotificationWindowModels : BindableBase
    {
        /// <summary>
        /// 弹出弹窗提示
        /// </summary>
        /// <param name="sets">NotificationSets 设置参数类</param>
        /// <param name="closeWindow"></param>
        public NotificationWindowModels(NotificationSets sets, Action closeWindow = null)
        {
            Title = sets.Title;
            Content = sets.Content;
            Button_Ok1 = sets.Button_Ok;
            Button_Cancal1 = sets.Button_Cancal;
            ShowTime = sets?.ShowTime > 0 ? sets.ShowTime : 0;

            CloseWindow = closeWindow;
            Button_Ok = new DelegateCommand(button_Ok_P);
            Button_Cancal = new DelegateCommand(button_Cancal_P);
        }
        private string showView = "ShowView";

        public string ShowView
        {
            get { return showView; }
            set { showView = value; RaisePropertyChanged(); }
        }


        private void button_Cancal_P()
        {
            ShowView = "";
            Button_Cancal1?.Invoke();
            CloseWindow?.Invoke();
        }

        private void button_Ok_P()
        {
            ShowView = "";
            Button_Ok1?.Invoke();
            CloseWindow?.Invoke();
        }

        public DelegateCommand Button_Ok { get; private set; }
        public DelegateCommand Button_Cancal { get; private set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }

        public Action Button_Ok1 { get; }
        public Action Button_Cancal1 { get; }
        public int ShowTime { get; }
        public Action CloseWindow { get; set; }
    }

    public class NotificationSets
    {
        /// <summary>
        /// 显示标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 显示内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// OK 后操作
        /// </summary>
        public Action Button_Ok { get; set; } = null;
        /// <summary>
        /// 取消 后操作
        /// </summary>
        public Action Button_Cancal { get; set; } = null;
        /// <summary>
        /// 显示保持时间
        /// </summary>
        public int ShowTime { get; set; }
    }
}
