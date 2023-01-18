using System;
using System.Collections.ObjectModel;
using Filer.Models.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class BookmarkPageViewModel : BindableBase, IDialogAware
    {
        private string keyText;
        private Mode mode;
        private string buttonText;

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

        public string KeyText { get => keyText; set => SetProperty(ref keyText, value); }

        public ObservableCollection<Favorite> Favorites { get; } = new ObservableCollection<Favorite>();

        public DelegateCommand ButtonCommand { get; private set; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult());
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            mode = parameters.GetValue<Mode>(nameof(Mode));

            if (mode == Mode.AdditionMode)
            {
                ButtonText = "ブックマークを追加";
                ButtonCommand = new DelegateCommand(() =>
                {
                    // ブックマークを追加する処理
                });
            }
            else if (mode == Mode.JumpMode)
            {
                ButtonText = "ブックマークにジャンプ";
                ButtonCommand = new DelegateCommand(() =>
                {
                    // ブックマークにジャンプする処理
                });
            }
        }
    }
}