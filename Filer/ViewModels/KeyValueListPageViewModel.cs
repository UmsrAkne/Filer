using System;
using System.Collections.Generic;
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

        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = string.Empty;

        public string KeyText { get => keyText; set => SetProperty(ref keyText, value); }

        public List<Favorite> Favorites { get; set; }

        public DelegateCommand ExecuteFromKeyCommand =>
            executeFromKeyCommand ?? (executeFromKeyCommand = new DelegateCommand(() =>
            {
            }));

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var setting = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
            Favorites = setting.Favorites;
        }
    }
}