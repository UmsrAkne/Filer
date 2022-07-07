namespace Filer.ViewModels
{
    using System;
    using Prism.Services.Dialogs;

    public class SelectionDialogViewModel : IDialogAware
    {
        public string Title => "SelectionDialog";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
