using System;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class InputPageViewModel : BindableBase, IDialogAware
    {
        private string message;
        private string inputText;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public string InputText
        {
            get => inputText;
            set => SetProperty(ref inputText, value);
        }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        private DirectoryInfo CurrentDirectoryInfo { get; set; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            CurrentDirectoryInfo = parameters.GetValue<DirectoryInfo>(nameof(DirectoryInfo));
            Message = parameters.GetValue<string>(nameof(Message));
        }
    }
}