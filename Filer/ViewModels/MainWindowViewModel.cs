using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private string title = "Prism Application";
        private ExtendFileInfo selectedItem;

        //// DelegateCommand *******************************************************

        private DelegateCommand openSettingFileCommand;
        private DelegateCommand showFavoritesCommand;
        private DelegateCommand<object> changeLayoutCommand;

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

            LeftFileListViewModel = new FileListViewModel(dialogService, OwnerListViewLocation.Left, Logger)
            {
                CurrentDirectory = new DirectoryInfo(leftWindowPath),
            };

            RightFileListViewModel = new FileListViewModel(dialogService, OwnerListViewLocation.Right, Logger)
            {
                CurrentDirectory = new DirectoryInfo(rightWindowPath),
            };
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public FileListViewModel LeftFileListViewModel { get; private set; }

        public FileListViewModel RightFileListViewModel { get; private set; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public bool TextInputting => GetFocusingListView().TextInputting;

        public Layout Layout { get; } = new Layout();

        public Logger Logger { get; } = new Logger();

        //// DelegateCommand *******************************************************

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

        public DelegateCommand OpenSettingFileCommand =>
            openSettingFileCommand ?? (openSettingFileCommand = new DelegateCommand(() =>
            {
                if (!File.Exists(ApplicationSetting.AppSettingFileName))
                {
                    File.Create(ApplicationSetting.AppSettingFileName);
                }

                Process.Start(ApplicationSetting.AppSettingFileName);
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

        public DelegateCommand<object> ChangeLayoutCommand =>
            changeLayoutCommand ?? (changeLayoutCommand = new DelegateCommand<object>((orientation) =>
            {
                if ((Orientation)orientation == Orientation.Horizontal)
                {
                    Layout.ToHorizontalLayout();
                }
                else
                {
                     Layout.ToVerticalLayout();
                }
            }));

        public DelegateCommand ExitCommand => new DelegateCommand(() =>
        {
            Application.Current.Shutdown();
        });

        public void DeleteFile()
        {
            var currentLv = GetFocusingListView();
            var targets = currentLv.FileList.Where(f => f.Marked).ToList();

            if (!targets.Any() || !targets.All(f => f.FileSystemInfo.Exists))
            {
                return;
            }

            var param = new DialogParameters { { nameof(YesNoPageViewModel.Message), "ファイルを削除します。よろしいですか？" } };
            var cancel = false;
            dialogService.ShowDialog(nameof(YesNoPage), param, result => cancel = result.Result == ButtonResult.No);

            if (cancel)
            {
                return;
            }

            foreach (var f in targets)
            {
                var deleteSuccess = true;

                try
                {
                    f.Delete();
                }
                catch (IOException)
                {
                    Logger.FailDelete(f.FileSystemInfo);
                    deleteSuccess = false;
                }

                if (deleteSuccess)
                {
                    Logger.FileDeleted(f.FileSystemInfo);
                }
            }

            currentLv.CurrentDirectory = currentLv.CurrentDirectory;
        }

        public void CopyFile()
        {
            var currentLv = GetFocusingListView();
            var anotherLv = GetAnotherListViewModel(currentLv);
            var targets = currentLv.FileList.Where(f => f.Marked).ToList();

            if (!targets.Any() || !targets.All(f => f.FileSystemInfo.Exists))
            {
                return;
            }

            foreach (var f in targets)
            {
                var copySuccess = true;

                try
                {
                    f.Copy(f.FileSystemInfo.FullName, $@"{anotherLv.CurrentDirectory.FullName}\{f.FileSystemInfo.Name}");
                }
                catch (IOException)
                {
                    Logger.FailCopy(f.FileSystemInfo);
                    copySuccess = false;
                }

                if (copySuccess)
                {
                    Logger.FileCopied(f.FileSystemInfo);
                }
            }

            currentLv.CurrentDirectory = currentLv.CurrentDirectory;
            anotherLv.CurrentDirectory = anotherLv.CurrentDirectory;
        }

        public void MoveFile()
        {
            var currentLv = GetFocusingListView();
            var anotherLv = GetAnotherListViewModel(currentLv);
            var targets = currentLv.FileList.Where(f => f.Marked).ToList();

            if (!targets.Any() || !targets.All(f => f.FileSystemInfo.Exists))
            {
                return;
            }

            foreach (var f in targets)
            {
                var moveSuccess = true;

                try
                {
                    f.Move(f.FileSystemInfo.FullName, $@"{anotherLv.CurrentDirectory.FullName}\{f.FileSystemInfo.Name}");
                }
                catch (IOException)
                {
                    Logger.FailMove(f.FileSystemInfo);
                    moveSuccess = false;
                }

                if (moveSuccess)
                {
                    Logger.FileMoved(f.FileSystemInfo);
                }
            }

            currentLv.CurrentDirectory = currentLv.CurrentDirectory;
            anotherLv.CurrentDirectory = anotherLv.CurrentDirectory;
        }

        public void FocusToListView(ListView lv)
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
        }

        public void SwitchFileListView()
        {
            (LeftFileListViewModel, RightFileListViewModel) = (RightFileListViewModel, LeftFileListViewModel);
            LeftFileListViewModel.IsFocused = false;
            RightFileListViewModel.IsFocused = false;

            RaisePropertyChanged(nameof(LeftFileListViewModel));
            RaisePropertyChanged(nameof(RightFileListViewModel));
        }

        public void SyncToAnother()
        {
            var currentLv = GetFocusingListView();
            GetAnotherListViewModel(currentLv).CurrentDirectory = currentLv.CurrentDirectory;
        }

        public void SyncFromAnother()
        {
            var currentLv = GetFocusingListView();
            currentLv.CurrentDirectory = GetAnotherListViewModel(currentLv).CurrentDirectory;
        }

        public void OpenFolderToAnotherListView()
        {
            var currentLv = GetFocusingListView();
            var dir = currentLv.SelectedItem;

            if (dir != null && dir.IsDirectory)
            {
                GetAnotherListViewModel(currentLv).CurrentDirectory = new DirectoryInfo(dir.FileSystemInfo.FullName);
            }
        }

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