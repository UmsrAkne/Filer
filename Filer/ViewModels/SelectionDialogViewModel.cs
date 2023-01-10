using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class SelectionDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand confirmCommand;
        private DelegateCommand cancelCommand;
        private DelegateCommand<TextBox> createDirectoryCommand;
        private DelegateCommand<TextBox> createFileCommand;
        private int selectedIndex = 0;
        private string inputName = "defaultName";
        private FileSystemInfo fileSystemInfo;

        public event Action<IDialogResult> RequestClose;

        public List<SelectionListItem> SelectableItems { get; set; } = new List<SelectionListItem>()
        {
            new SelectionListItem() { Header = "Directory", Key = Key.D, Index = 1 },
            new SelectionListItem() { Header = "Empty File", Key = Key.F, Index = 2 },
        };

        public string InputName { get => inputName; set => SetProperty(ref inputName, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public string Title => "SelectionDialog";

        public DelegateCommand ConfirmCommand =>
            confirmCommand ?? (confirmCommand = new DelegateCommand(() =>
            {
                var fileName = $@"{fileSystemInfo.FullName}\{InputName}";

                // パスに無効な文字が含まれていないか
                if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    RequestClose?.Invoke(new DialogResult());
                    return;
                }

                // 同名のファイルかディレクトリが存在していないか
                if (Directory.Exists(fileName) || File.Exists(fileName))
                {
                    RequestClose?.Invoke(new DialogResult());
                    return;
                }

                if (SelectedIndex == 0)
                {
                    Directory.CreateDirectory(fileName);
                }
                else
                {
                    File.Create(fileName).Close();
                }

                RequestClose?.Invoke(new DialogResult());
            }));

        public DelegateCommand CancelCommand =>
            cancelCommand ?? (cancelCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult());
            }));

        public DelegateCommand<TextBox> CreateDirectoryCommand =>
            createDirectoryCommand ?? (createDirectoryCommand = new DelegateCommand<TextBox>((tb) =>
            {
                SelectedIndex = 0;
                Keyboard.Focus(tb);
            }));

        public DelegateCommand<TextBox> CreateFileCommand =>
            createFileCommand ?? (createFileCommand = new DelegateCommand<TextBox>((tb) =>
            {
                SelectedIndex = 1;
                Keyboard.Focus(tb);
            }));

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            fileSystemInfo = parameters.GetValue<FileSystemInfo>(nameof(FileSystemInfo));
        }
    }
}