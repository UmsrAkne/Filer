using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDialogAware, IListViewControllable
    {
        private DelegateCommand<ListView> scrollCommand;

        private ObservableCollection<ExtendFileInfo> histories;
        private string commandText = string.Empty;
        private DelegateCommand<TextBox> focusCommandTextBoxCommand;
        private DelegateCommand<TextBox> startPartialMatchSearchCommand;
        private DelegateCommand<ListView> searchFileCommand;
        private DelegateCommand<ListView> reverseSearchFileCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public double ListViewItemLineHeight => 15.0;

        public string CommandText { get => commandText; set => SetProperty(ref commandText, value); }

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

        public DelegateCommand<TextBox> FocusCommandTextBoxCommand =>
            focusCommandTextBoxCommand ?? (focusCommandTextBoxCommand = new DelegateCommand<TextBox>(t =>
            {
                t.Focus();
                t.Text = "^.*";
                t.SelectionStart = 1;
            }));

        public DelegateCommand<TextBox> StartPartialMatchSearchCommand =>
            startPartialMatchSearchCommand ?? (startPartialMatchSearchCommand = new DelegateCommand<TextBox>(t =>
            {
                t.Focus();
                t.Text = string.Empty;
            }));

        public DelegateCommand<ListView> SearchFileCommand =>
            searchFileCommand ?? (searchFileCommand = new DelegateCommand<ListView>((lv) =>
            {
                FileContainer.JumpToNextFileName(CommandText, new Logger());
                if (lv.Items.Count > 0)
                {
                    var item =
                        lv.ItemContainerGenerator.ContainerFromIndex(FileContainer.SelectedIndex) as ListViewItem;
                    Keyboard.Focus(item);
                }
            }));

        public DelegateCommand<ListView> ReverseSearchFileCommand =>
            reverseSearchFileCommand ?? (reverseSearchFileCommand = new DelegateCommand<ListView>((lv) =>
            {
                FileContainer.JumpToPrevFileName(CommandText, new Logger());
                if (lv.Items.Count > 0)
                {
                    var item =
                        lv.ItemContainerGenerator.ContainerFromIndex(FileContainer.SelectedIndex) as ListViewItem;
                    Keyboard.Focus(item);
                }

                var index = FileContainer.SelectedIndex;

                foreach (var f in Histories.Where(f => f.IsSelected))
                {
                    f.IsSelected = false;
                }

                FileContainer.SelectedIndex = index;
            }));

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
        }
    }
}