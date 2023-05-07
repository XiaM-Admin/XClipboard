using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using XClipboard.Common;
using XClipboard.Common.Models;

namespace XClipboard.ViewModels
{
    public class HomeViewModels : BindableBase
    {
        private ObservableCollection<ClipboardbLists> clipboards;
        private ObservableCollection<ListsModels> lists;
        private object newSelectedValue;
        private ObservableCollection<ProgramState> programStates;

        /// <summary>
        /// CTOR 快速生成构造函数
        /// </summary>
        public HomeViewModels(IDialogService dialogService)
        {
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            if (appState.IsClipboardServiceRunning.LocalStorage && appState.IsClipboardServiceRunning.Clipboard)
            {
                appState.IsClipboardServiceRunning.IsRunTrue = true;
                appState.IsClipboardServiceRunning.Color = "#36BF36";
                appState.IsClipboardServiceRunning.Text = "运行正常";
            }
            else
                appState.IsClipboardServiceRunning.Text = "初始化失败";

            ProgramStates = new ObservableCollection<ProgramState>()
            {
                appState.IsClipboardServiceRunning
            };
            Lists = new ObservableCollection<ListsModels>();
            BindShowLists();
            Clipboards = new ObservableCollection<ClipboardbLists>();
            BindingClipboards();

            OpenInfoDialog = new DelegateCommand(Open);
            MouseDoubleClick = new DelegateCommand<object>(mouseDoubleClick);

            DialogService = dialogService;
        }

        public ObservableCollection<ClipboardbLists> Clipboards
        {
            get { return clipboards; }
            set { clipboards = value; RaisePropertyChanged(); }
        }

        public IDialogService DialogService { get; }

        public ObservableCollection<ListsModels> Lists
        {
            get { return lists; }
            set { lists = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<object> MouseDoubleClick { get; private set; }

        public object NewSelectedValue
        {
            get { return newSelectedValue; }
            set { newSelectedValue = value; RaisePropertyChanged(); }
        }

        public DelegateCommand OpenInfoDialog { get; private set; }

        public ObservableCollection<ProgramState> ProgramStates
        {
            get { return programStates; }
            set { programStates = value; RaisePropertyChanged(); }
        }

        private void BindingClipboards()
        {
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            Clipboards = appState.NewClipboards;
        }

        private void BindShowLists()
        {
            var appState = (AppState)System.Windows.Application.Current.Properties["AppState"];
            Lists = appState.ClipboardListNumber;
        }

        private void mouseDoubleClick(object obj)
        {
            if (NewSelectedValue is null) return;
            Clipboardb_Models list = NewSelectedValue as Clipboardb_Models;

            DialogParameters keyValues = new()
                {
                    {"content", new ClipboardItems(list) }
                };
            DialogService.ShowDialog("TextShowView", keyValues, t => { });
        }

        private void Open()
        {
            //弹出Dialog窗口
            DialogService.ShowDialog("ProgramInfoView");
        }
    }
}