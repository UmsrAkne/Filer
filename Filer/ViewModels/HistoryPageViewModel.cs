using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public double ListViewItemLineHeight => 15.0;

        public ObservableCollection<ExtendFileInfo> Histories
        {
            get => histories;
            private set => SetProperty(ref histories, value);
        }

        public DelegateCommand<ListView> ScrollCommand =>
            scrollCommand ?? (scrollCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (lv.SelectedItem != null)
                {
                    lv.ScrollIntoView(lv.SelectedItem);
                }
            }));

        public DelegateCommand ConfirmCommand => new DelegateCommand(() =>
        {
            var result = new DialogResult();
            if (FileContainer.SelectedItem != null)
            {
                result.Parameters.Add(nameof(ExtendFileInfo), FileContainer.SelectedItem);
            }

            RequestClose?.Invoke(result);
        });

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public FileContainer FileContainer { get; set; } = new FileContainer();

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            using (var context = new DatabaseContext())
            {
                Histories = new ObservableCollection<ExtendFileInfo>(
                    context.GetHistories().Select(h => new ExtendFileInfo(h.Path)));
            }

            FileContainer.Files = Histories;
            CursorMoveCommands = new CursorMoveCommands
            {
                FileContainer = FileContainer,
                ListViewItemLineHeight = 15.0,
            };
        }
    }
}