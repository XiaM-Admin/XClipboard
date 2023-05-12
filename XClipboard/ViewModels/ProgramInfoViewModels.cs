using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using XClipboard.Common;

namespace XClipboard.ViewModels
{
    public class ProgramInfoViewModels : BindableBase, IDialogAware
    {
        private string clipborad;

        private string localStorage;

        private string runTime;

        public ProgramInfoViewModels()
        {
            var appState = (AppState)Application.Current.Properties["AppState"];
            if (appState.IsClipboardServiceRunning != null)
            {
                if (appState.IsClipboardServiceRunning.Clipboard)
                    Clipborad = "True";
                else
                    Clipborad = "False 设置中可能处于关闭状态";

                if (appState.IsClipboardServiceRunning.LocalStorage)
                    LocalStorage = "True";
                else
                    LocalStorage = "False";
            }
            RunTime = appState.GetTotalRunningTime().ToString();
        }

        public event Action<IDialogResult> RequestClose;

        public string Clipborad
        {
            get { return clipborad; }
            set { clipborad = value; RaisePropertyChanged(); }
        }

        public string LocalStorage
        {
            get { return localStorage; }
            set { localStorage = value; RaisePropertyChanged(); }
        }

        public string RunTime
        {
            get { return runTime; }
            set { runTime = value; RaisePropertyChanged(); }
        }

        public string Title => "详情";

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}