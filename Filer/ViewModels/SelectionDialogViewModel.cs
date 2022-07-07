namespace Filer.ViewModels
{
    using System;
    using Prism.Commands;
    using Prism.Services.Dialogs;

    public class SelectionDialogViewModel : IDialogAware
    {
        private DelegateCommand confirmCommand;
        private DelegateCommand cancelCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title => "SelectionDialog";

        public DelegateCommand ConfirmCommand
        {
            get => confirmCommand ?? (confirmCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult());
            }));
        }

        public DelegateCommand CancelCommand
        {
            get => cancelCommand ?? (cancelCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult());
            }));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
