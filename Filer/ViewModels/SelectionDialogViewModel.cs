namespace Filer.ViewModels
{
    using System;
    using Prism.Services.Dialogs;

    public class SelectionDialogViewModel : IDialogAware
    {
        public string Title => "SelectionDialog";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
