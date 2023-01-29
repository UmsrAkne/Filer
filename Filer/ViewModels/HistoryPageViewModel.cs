using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<ListView> scrollCommand;

        private ObservableCollection<ExtendFileInfo> histories;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public CursorMoveCommands CursorMoveCommands { get; private set; }

        public ObservableCollection<ExtendFileInfo> Histories { get => histories; set => SetProperty(ref histories, value); }

        public DelegateCommand<ListView> ScrollCommand =>
            scrollCommand ?? (scrollCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (lv.SelectedItem != null)
                {
                    lv.ScrollIntoView(lv.SelectedItem);
                }
            }));

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
            CursorMoveCommands = new CursorMoveCommands
            {
                FileContainer = new FileContainer()
                {
                    Files = Histories,
                },
                ListViewItemLineHeight = 15.0,
            };
        }
    }
}