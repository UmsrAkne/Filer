using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class BookmarkPageViewModel : BindableBase, IDialogAware
    {
        private string keyText;

        public event Action<IDialogResult> RequestClose;

        public string Title => "Bookmarks";

        public string KeyText { get => keyText; set => SetProperty(ref keyText, value); }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}