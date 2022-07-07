namespace Filer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    public class SelectionDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand confirmCommand;
        private DelegateCommand cancelCommand;
        private DelegateCommand<TextBox> createDirectoryCommand;
        private DelegateCommand<TextBox> createFileCommand;
        private int selectedIndex = 0;
        private string inputName = "defaultName";

        public event Action<IDialogResult> RequestClose;

        public List<SelectionListItem> SelectableItems { get; set; } = new List<SelectionListItem>()
        {
            new SelectionListItem() { Header = "Directory", Key = Key.D, Index = 1 },
            new SelectionListItem() { Header = "Empty File", Key = Key.F, Index = 2 },
        };

        public string InputName { get => inputName; set => SetProperty(ref inputName, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

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

        public DelegateCommand<TextBox> CreateDirectoryCommand
        {
            get => createDirectoryCommand ?? (createDirectoryCommand = new DelegateCommand<TextBox>((tb) =>
            {
                SelectedIndex = 0;
                Keyboard.Focus(tb);
            }));
        }

        public DelegateCommand<TextBox> CreateFileCommand
        {
            get => createFileCommand ?? (createFileCommand = new DelegateCommand<TextBox>((tb) =>
            {
                SelectedIndex = 1;
                Keyboard.Focus(tb);
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
