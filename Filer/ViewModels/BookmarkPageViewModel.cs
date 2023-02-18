using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Filer.Models;
using Filer.Models.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BookmarkPageViewModel : BindableBase, IDialogAware
    {
        private string keyText;
        private Mode mode;
        private string buttonText;
        private Visibility listViewVisibility;

        public event Action<IDialogResult> RequestClose;

        public enum Mode
        {
            #pragma warning disable SA1602
            AdditionMode,
            JumpMode,
            #pragma warning restore SA1602
        }

        public string Title => "Bookmarks";

        public string ButtonText
        {
            get => buttonText;
            private set => SetProperty(ref buttonText, value);
        }

        public string KeyText
        {
            get => keyText;
            set
            {
                if (mode == Mode.JumpMode)
                {
                    foreach (var f in Favorites)
                    {
                        f.IsMatch = value != string.Empty && f.Key.StartsWith(value);
                    }
                }

                SetProperty(ref keyText, value);
            }
        }

        public ObservableCollection<Favorite> Favorites { get; private set; }

        public Visibility ListViewVisibility
        {
            get => listViewVisibility;
            set => SetProperty(ref listViewVisibility, value);
        }

        public DelegateCommand ButtonCommand { get; private set; }

        public DelegateCommand CancelCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            mode = parameters.GetValue<Mode>(nameof(Mode));
            if (mode == Mode.AdditionMode)
            {
                ButtonText = "ブックマークを追加 (Ctrl + Enter)";
                ListViewVisibility = Visibility.Collapsed;
                ButtonCommand = new DelegateCommand(() =>
                    AddBookmark(parameters.GetValue<ExtendFileInfo>(nameof(ExtendFileInfo))));
            }
            else if (mode == Mode.JumpMode)
            {
                var settings = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
                Favorites = new ObservableCollection<Favorite>(settings.Bookmarks);

                ButtonText = "ブックマークにジャンプ (Ctrl + Enter)";
                ListViewVisibility = Visibility.Visible;
                ButtonCommand = new DelegateCommand(ReturnResult);
            }
        }

        private void ReturnResult()
        {
            if (Favorites.Count == 0 || string.IsNullOrWhiteSpace(KeyText))
            {
                return;
            }

            var target = Favorites.FirstOrDefault(f => f.IsMatch);
            var buttonResult = target != null ? ButtonResult.OK : ButtonResult.Cancel;

            RequestClose?.Invoke(new DialogResult(buttonResult, new DialogParameters
            {
                { nameof(Favorite), target },
            }));
        }

        private void AddBookmark(ExtendFileInfo file)
        {
            if (file == null || string.IsNullOrWhiteSpace(KeyText))
            {
                return;
            }

            var fav = new Favorite()
            {
                Name = file.Name,
                Path = file.FileSystemInfo.FullName,
                Key = KeyText,
            };

            var settings = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
            settings.Bookmarks.Add(fav);
            ApplicationSetting.WriteApplicationSetting(settings);
            RequestClose?.Invoke(new DialogResult());
        }
    }
}