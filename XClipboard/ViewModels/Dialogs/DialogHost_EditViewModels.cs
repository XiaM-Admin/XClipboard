using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace XClipboard.ViewModels.Dialogs
{
    internal class DialogHost_EditViewModels : BindableBase, IDialogAware
    {
        private string content;
        private ButtonResult dialogResult;
        private string tip;

        public DialogHost_EditViewModels()
        {
            Button_Click = new DelegateCommand<string>(button_Click);
        }

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<string> Button_Click { private set; get; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Tip
        {
            get { return tip; }
            set { tip = value; RaisePropertyChanged(); }
        }

        public string Title => "Edit Dialog";

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            DialogParameters keyValuePairs = new()
            {
                {"Content",Content }
            };
            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(dialogResult, keyValuePairs));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters is not null)
            {
                Tip = parameters.ContainsKey("Tip") ? parameters.GetValue<string>("Tip") : "";
                Content = parameters.ContainsKey("Content") ? parameters.GetValue<string>("Content") : "";
            }
        }

        private void button_Click(string obj)
        {
            switch (obj)
            {
                case "True":
                    dialogResult = ButtonResult.OK;
                    break;

                case "False":
                    dialogResult = ButtonResult.Cancel;
                    break;

                default:
                    break;
            }
            this.OnDialogClosed();
        }
    }
}