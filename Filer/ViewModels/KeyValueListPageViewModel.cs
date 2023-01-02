using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Filer.Models.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class KeyValueListPageViewModel : BindableBase, IDialogAware
    {
        private string keyText;

        private DelegateCommand executeFromKeyCommand;
        private DelegateCommand closeCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = "Favorites";

        public string KeyText
        {
            get => keyText;
            set
            {
                foreach (var f in Favorites)
                {
                    f.IsMatch = value != string.Empty && f.Key.StartsWith(value);
                }

                SetProperty(ref keyText, value);
            }
        }

        public ObservableCollection<Favorite> Favorites { get; set; }

        public DelegateCommand ExecuteFromKeyCommand =>
            executeFromKeyCommand ?? (executeFromKeyCommand = new DelegateCommand(() =>
            {
                var targetPath = Favorites.FirstOrDefault(f => f.IsMatch)?.Path;
                KeyText = string.Empty;

                if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
                {
                    return;
                }

                if (Directory.Exists(targetPath))
                {
                    var result = new DialogResult();
                    result.Parameters.Add(nameof(FileSystemInfo), new DirectoryInfo(targetPath));
                    RequestClose?.Invoke(result);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(targetPath))
                {
                    Process.Start(targetPath);
                }
            }));

        public DelegateCommand CloseCommand =>
            closeCommand ?? (closeCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(null);
            }));

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var setting = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
            Favorites = new ObservableCollection<Favorite>(setting.Favorites);
        }
    }
}