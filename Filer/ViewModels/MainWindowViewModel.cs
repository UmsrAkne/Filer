using System;
using System.Collections.Generic;
using System.IO;
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
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private string title = "Prism Application";
        private ExtendFileInfo selectedItem;

        //// DelegateCommand *******************************************************

        private DelegateCommand<ListView> focusToListViewCommand;
        private DelegateCommand showFavoritesCommand;
        private DelegateCommand switchFileListVmCommand;
        private DelegateCommand syncToAnotherCommand;
        private DelegateCommand syncFromAnotherCommand;

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

        public FileListViewModel LeftFileListViewModel { get; private set; }

        public FileListViewModel RightFileListViewModel { get; private set; }

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

        public DelegateCommand SwitchFileListVmCommand =>
            switchFileListVmCommand ?? (switchFileListVmCommand = new DelegateCommand(() =>
            {
                (LeftFileListViewModel, RightFileListViewModel) = (RightFileListViewModel, LeftFileListViewModel);
                LeftFileListViewModel.IsFocused = false;
                RightFileListViewModel.IsFocused = false;

                RaisePropertyChanged(nameof(LeftFileListViewModel));
                RaisePropertyChanged(nameof(RightFileListViewModel));
            }));

        public DelegateCommand SyncToAnotherCommand =>
            syncToAnotherCommand ?? (syncToAnotherCommand = new DelegateCommand(() =>
            {
                var currentLv = GetFocusingListView();
                GetAnotherListViewModel(currentLv).CurrentDirectory = currentLv.CurrentDirectory;
            }));

        public DelegateCommand SyncFromAnotherCommand =>
            syncFromAnotherCommand ?? (syncFromAnotherCommand = new DelegateCommand(() =>
            {
                var currentLv = GetFocusingListView();
                currentLv.CurrentDirectory = GetAnotherListViewModel(currentLv).CurrentDirectory;
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

        /// <summary>
        /// LeftFileListViewModel, RightFileListViewModel のいずれかを入力して使用します。
        /// 入力したほうとは別のビューモデルを返します。
        /// </summary>
        /// <param name="vm">LeftFileListViewModel, RightFileListViewModel　のいずれかを入力します</param>
        /// <returns>入力したビューモデルとは別の方のビューモデルを返します</returns>
        /// <exception cref="ArgumentException">param で指定されているビューモデル以外のインスタンスが入力された時スローされます</exception>
        private FileListViewModel GetAnotherListViewModel(FileListViewModel vm)
        {
            if (LeftFileListViewModel == vm)
            {
                return RightFileListViewModel;
            }

            if (RightFileListViewModel == vm)
            {
                return LeftFileListViewModel;
            }

            throw new ArgumentException("不正なビューモデル、または Null が入力されました");
        }
    }
}