using System;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class SortSettingPageViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public SortStatus SortStatus { get; set; }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand<SortStatus> ConfirmCommand => new DelegateCommand<SortStatus>((sortStatus) =>
        {
            SortStatus.Key = sortStatus.Key;
            SortStatus.Reverse = sortStatus.Reverse;
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand ReverseCommand => new DelegateCommand(() =>
        {
            SortStatus.Reverse = !SortStatus.Reverse;
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            SortStatus = parameters.GetValue<SortStatus>(nameof(SortStatus));
        }
    }
}