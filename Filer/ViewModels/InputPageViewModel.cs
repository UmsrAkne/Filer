using System;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InputPageViewModel : BindableBase, IDialogAware
    {
        private string message;
        private string inputText;
        private DelegateCommand confirmCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public string InputText
        {
            get => inputText;
            set => SetProperty(ref inputText, value);
        }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand ConfirmCommand =>
            confirmCommand ?? (confirmCommand = new DelegateCommand(() =>
            {
                var fileName = $@"{CurrentDirectoryInfo.FullName}\{InputText}";

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

                switch (FileSystemInfo)
                {
                    case DirectoryInfo _:
                        Directory.CreateDirectory(fileName);
                        break;

                    case FileInfo _:
                        File.Create(fileName).Close();
                        break;
                }

                RequestClose?.Invoke(new DialogResult());
            }));

        private DirectoryInfo CurrentDirectoryInfo { get; set; }

        /// <summary>
        /// ページ内で作成するファイルがディレクトリかファイルかを表すプロパティです
        /// is FileInfo ならファイル is DirectoryInfo ならディレクトリです。
        /// </summary>
        private FileSystemInfo FileSystemInfo { get; set; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            CurrentDirectoryInfo = parameters.GetValue<DirectoryInfo>(nameof(DirectoryInfo));
            Message = parameters.GetValue<string>(nameof(Message));
            FileSystemInfo = parameters.GetValue<FileSystemInfo>(nameof(FileSystemInfo));

            if (FileSystemInfo == null || CurrentDirectoryInfo == null)
            {
                throw new ArgumentException("FileSystemInfo または CurrentDirectoryInfo のいずれかが null です。");
            }
        }
    }
}