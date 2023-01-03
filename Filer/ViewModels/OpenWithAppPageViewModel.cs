using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Filer.Models.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OpenWithAppPageViewModel : BindableBase, IDialogAware
    {
        private string keyText;

        private DelegateCommand executeFromKeyCommand;
        private DelegateCommand closeCommand;
        private string message = string.Empty;

        public event Action<IDialogResult> RequestClose;

        public string Title => "Applications";

        public string KeyText
        {
            get => keyText;
            set
            {
                foreach (var f in Applications)
                {
                    f.IsMatch = value != string.Empty && f.Key.StartsWith(value);
                }

                SetProperty(ref keyText, value);
            }
        }

        public ObservableCollection<Favorite> Applications { get; private set; }

        public DelegateCommand ExecuteFromKeyCommand =>
            executeFromKeyCommand ?? (executeFromKeyCommand = new DelegateCommand(() =>
            {
                var targetPath = Applications.FirstOrDefault(f => f.IsMatch)?.Path;
                KeyText = string.Empty;

                if (!string.IsNullOrWhiteSpace(targetPath) && !File.Exists(targetPath))
                {
                    return;
                }

                var result = new DialogResult();
                result.Parameters.Add(nameof(FileInfo), new FileInfo(targetPath));
                RequestClose?.Invoke(result);
            }));

        public DelegateCommand CloseCommand =>
            closeCommand ?? (closeCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(null);
            }));

        public string Message { get => message; set => SetProperty(ref message, value); }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var setting = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
            Applications = new ObservableCollection<Favorite>(setting.Apps);
            var openFileCount = parameters.GetValue<int>("OpenFileCount");
            Message = $"{openFileCount} 個のファイルを開きます。アプリケーションを指定してください。";
        }
    }
}