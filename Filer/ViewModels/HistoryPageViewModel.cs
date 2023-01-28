using System;
using System.Collections.ObjectModel;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDialogAware
    {
        private ObservableCollection<History> histories;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public ObservableCollection<History> Histories { get => histories; set => SetProperty(ref histories, value); }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}