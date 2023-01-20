using System.IO;
using System.Windows;
using Filer.Models.Settings;
using Filer.ViewModels;
using Filer.Views;
using Prism.Ioc;

namespace Filer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SelectionDialog, SelectionDialogViewModel>();
            containerRegistry.RegisterDialog<KeyValueListPage, KeyValueListPageViewModel>();
            containerRegistry.RegisterDialog<OpenWithAppPage, OpenWithAppPageViewModel>();
            containerRegistry.RegisterDialog<BookmarkPage, BookmarkPageViewModel>();
            containerRegistry.RegisterDialog<SortSettingPage, SortSettingPageViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!File.Exists(ApplicationSetting.AppSettingFileName))
            {
                var defaultSettings = new ApplicationSetting();

                // 設定項目のサンプルとして、デフォルトでメモ帳を登録しておく
                defaultSettings.Favorites.Add(new Favorite()
                {
                    // ReSharper disable once StringLiteralTypo, notepad が正しい単語として認識されないため
                    Path = @"C:\Windows\notepad.exe",
                    Name = "Notepad",
                    Key = "n",
                });

                defaultSettings.Apps.Add(new Favorite
                {
                    // ReSharper disable once StringLiteralTypo, notepad が正しい単語として認識されないため
                    Path = @"C:\Windows\notepad.exe",
                    Name = "Notepad",
                    Key = "note",
                });

                ApplicationSetting.WriteApplicationSetting(defaultSettings);
            }

            base.OnStartup(e);
        }
    }
}