using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Filer.Models.Settings;
using Filer.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private IDialogService dialogService;
        private ExtendFileInfo selectedItem;

        //// DelegateCommand *******************************************************

        private DelegateCommand<ListView> focusToListViewCommand;
        private DelegateCommand showFavoritesCommand;

        public MainWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            // アプリケーション起動時の最初のディレクトリを設定する。
            // 前回終了時の情報があって、使えるならそれを優先する。使えない場合はユーザールートをデフォルトとする。
            string leftWindowPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string rightWindowPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var setting = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);

            if (setting.LastVisitedDirectories.Count >= 2)
            {
                if (Directory.Exists(setting.LastVisitedDirectories[0]))
                {
                    leftWindowPath = setting.LastVisitedDirectories[0];
                }

                if (Directory.Exists(setting.LastVisitedDirectories[1]))
                {
                    rightWindowPath = setting.LastVisitedDirectories[1];
                }
            }

            LeftFileListViewModel = new FileListViewModel(dialogService)
            {
                OwnerListViewLocation = OwnerListViewLocation.Left,
                Logger = Logger,
                CurrentDirectory = new DirectoryInfo(leftWindowPath),
            };

            RightFileListViewModel = new FileListViewModel(dialogService)
            {
                OwnerListViewLocation = OwnerListViewLocation.Right,
                Logger = Logger,
                CurrentDirectory = new DirectoryInfo(rightWindowPath),
            };
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public FileListViewModel LeftFileListViewModel { get; }

        public FileListViewModel RightFileListViewModel { get; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public Logger Logger { get; } = new Logger();

        //// DelegateCommand *******************************************************

        public DelegateCommand<ListView> FocusToListViewCommand =>
            focusToListViewCommand ?? (focusToListViewCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (lv.Items.Count == 0)
                {
                    Keyboard.Focus(lv);
                }
                else
                {
                    var destIndex = lv.SelectedIndex < 0 ? 0 : lv.SelectedIndex;
                    lv.SelectedIndex = destIndex;
                    var item = lv.ItemContainerGenerator.ContainerFromIndex(destIndex) as ListViewItem;
                    Keyboard.Focus(item);
                }
            }));

        public DelegateCommand ShowFavoritesCommand =>
            showFavoritesCommand ?? (showFavoritesCommand = new DelegateCommand(() =>
            {
                dialogService.ShowDialog(nameof(KeyValueListPage), new DialogParameters(), result =>
                {
                    if (result.Parameters.ContainsKey(nameof(FileSystemInfo)))
                    {
                        var vm = GetFocusingListView();
                        vm.CurrentDirectory = result.Parameters.GetValue<DirectoryInfo>(nameof(FileSystemInfo));
                    }
                });
            }));

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            var setting = ApplicationSetting.ReadApplicationSetting(ApplicationSetting.AppSettingFileName);
            setting.LastVisitedDirectories = new List<string>
            {
                LeftFileListViewModel.CurrentDirectory.FullName,
                RightFileListViewModel.CurrentDirectory.FullName,
            };

            ApplicationSetting.WriteApplicationSetting(setting);
        });

        /// <summary>
        /// 現在フォーカスのあるファイルリストビューのビューモデルを返します。
        /// どっちにもフォーカスがない場合は、今のところ左側のファイルリストビューモデルを返します
        /// </summary>
        /// <returns>フォーカスのあるファイルリストビューモデル</returns>
        private FileListViewModel GetFocusingListView()
        {
            if (LeftFileListViewModel.IsFocused)
            {
                return LeftFileListViewModel;
            }

            if (RightFileListViewModel.IsFocused)
            {
                return RightFileListViewModel;
            }

            return LeftFileListViewModel;
        }
    }
}