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
        /// <param name="title">标题</param>
        /// <param name="content">主题内容</param>
        /// <param name="button_Ok">确定事件</param>
        /// <param name="button_Cancal">取消事件</param>
        public NotificationWindowModels(string title, string content, Action button_Ok = null, Action button_Cancal = null, Action closeWindow = null)
        {
            Title = title;
            Content = content;

            Button_Ok1 = button_Ok;
            Button_Cancal1 = button_Cancal;
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
        public Action CloseWindow { get; set; }
    }
}
