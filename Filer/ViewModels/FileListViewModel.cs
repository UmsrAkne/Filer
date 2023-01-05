using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Filer.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class FileListViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private bool isFocused;
        private string pathBarText;
        private string commandText;
        private int selectedIndex;
        private ObservableCollection<ExtendFileInfo> fileList;
        private DirectoryInfo currentDirectory;
        private double listViewItemLineHeight = 15.0;
        private int executeCounter;
        private DelegateCommand<string> openPathCommand;
        private DelegateCommand<ListView> openFileCommand;
        private DelegateCommand openWithAppCommand;
        private DelegateCommand<ListView> cursorDownCommand;
        private DelegateCommand<ListView> cursorUpCommand;
        private DelegateCommand jumpToFirstCommand;
        private DelegateCommand<ListView> jumpToLastCommand;
        private DelegateCommand directoryUpCommand;
        private DelegateCommand<ListView> pageUpCommand;
        private DelegateCommand<ListView> pageDownCommand;
        private DelegateCommand createCommand;
        private DelegateCommand markCommand;
        private DelegateCommand<ListView> markAndDownCommand;

        private ExtendFileInfo selectedItem;

        public FileListViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public OwnerListViewLocation OwnerListViewLocation { get; set; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public bool IsFocused { get => isFocused; set => SetProperty(ref isFocused, value); }

        public string PathBarText { get => pathBarText; set => SetProperty(ref pathBarText, value); }

        public string CommandText { get => commandText; set => SetProperty(ref commandText, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public ObservableCollection<ExtendFileInfo> FileList { get => fileList; private set => SetProperty(ref fileList, value); }

        public double ListViewItemLineHeight { get => listViewItemLineHeight; private set => SetProperty(ref listViewItemLineHeight, value); }

        public int ExecuteCounter { get => executeCounter; set => SetProperty(ref executeCounter, value); }

        public Logger Logger { private get; set; }

        public DirectoryInfo CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                try
                {
                    value.GetDirectories();
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.FailAccess(value);
                    return;
                }

                var oldDirectory = currentDirectory;
                currentDirectory = value;

                FileList = GetFileList(value.FullName, OwnerListViewLocation);
                PathBarText = value.FullName;

                if (oldDirectory == null)
                {
                    return;
                }

                if (oldDirectory.FullName != value.FullName)
                {
                    Logger.ChangeCurrentDirectoryLog(oldDirectory, value);
                }
                else
                {
                    Logger.ReloadDirectory(oldDirectory, OwnerListViewLocation);
                }
            }
        }

        public DelegateCommand<string> OpenPathCommand =>
            openPathCommand ?? (openPathCommand = new DelegateCommand<string>((locationString) =>
            {
                CurrentDirectory = new DirectoryInfo(PathBarText);
            }));

        public DelegateCommand<ListView> OpenFileCommand =>
            openFileCommand ?? (openFileCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsDirectory)
                    {
                        CurrentDirectory = SelectedItem.FileSystemInfo as DirectoryInfo;

                        if (lv.Items.Count > 0)
                        {
                            SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        Process.Start(SelectedItem.FileSystemInfo.FullName);
                    }
                }
            }));

        public DelegateCommand OpenWithAppCommand =>
            openWithAppCommand ?? (openWithAppCommand = new DelegateCommand(() =>
            {
                List<ExtendFileInfo> targets = new List<ExtendFileInfo>();

                // マークされているファイルがある場合はそれを優先する
                if (FileList.Any(f => f.Marked))
                {
                    targets = FileList.Where(f => f.Marked).ToList();
                }
                else if (SelectedItem != null)
                {
                    // マークがされていない場合は、現在カーソルが当たっているファイルを対象とする
                    targets = new List<ExtendFileInfo>() { SelectedItem };
                }

                if (targets.Count != 0)
                {
                    var param = new DialogParameters { { "OpenFileCount", targets.Count() } };
                    dialogService.ShowDialog(nameof(OpenWithAppPage), param, (IDialogResult dialogResult) =>
                    {
                        if (dialogResult.Parameters.ContainsKey(nameof(FileInfo)))
                        {
                            var executeFilePath = dialogResult.Parameters.GetValue<FileInfo>(nameof(FileInfo));
                            foreach (var f in targets)
                            {
                                Process.Start(executeFilePath.FullName, f.FileSystemInfo.FullName);
                            }
                        }
                    });
                }
            }));

        public DelegateCommand<ListView> CursorDownCommand =>
            cursorDownCommand ?? (cursorDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                MoveCursor(lv, amount);
            }));

        public DelegateCommand<ListView> CursorUpCommand =>
            cursorUpCommand ?? (cursorUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter * -1 : -1;
                ExecuteCounter = 0;
                MoveCursor(lv, amount);
            }));

        public DelegateCommand<ListView> JumpToLastCommand =>
            jumpToLastCommand ?? (jumpToLastCommand = new DelegateCommand<ListView>((lv) =>
            {
                var currentIndex = lv.SelectedIndex == -1 ? 0 : lv.SelectedIndex;
                MoveCursor(lv, FileList.Count() + 1);

                // 最後の行までジャンプした直後に ListViewItem が範囲選択されるので、選択状態をリセットしている。
                var item = lv.SelectedItem;
                FileList.Skip(currentIndex).ToList().ForEach(f => f.IsSelected = false);
                SelectedItem = item as ExtendFileInfo;
            }));

        public DelegateCommand JumpToFirstCommand =>
            jumpToFirstCommand ?? (jumpToFirstCommand = new DelegateCommand(() =>
            {
                if (SelectedIndex != -1)
                {
                    SelectedIndex = 0;
                }
            }));

        public DelegateCommand DirectoryUpCommand =>
            directoryUpCommand ?? (directoryUpCommand = new DelegateCommand(() =>
            {
                var parentDirectoryInfo = new DirectoryInfo(CurrentDirectory.FullName).Parent;

                if (parentDirectoryInfo != null)
                {
                    CurrentDirectory = parentDirectoryInfo;
                }
            }));

        public DelegateCommand<ListView> PageUpCommand =>
            pageUpCommand ?? (pageUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                for (var i = 0; i < amount; i++)
                {
                    MoveCursor(lv, (int)(lv.ActualHeight / (ListViewItemLineHeight + 8)) * -1);
                }
            }));

        public DelegateCommand<ListView> PageDownCommand =>
            pageDownCommand ?? (pageDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                for (var i = 0; i < amount; i++)
                {
                    MoveCursor(lv, (int)(lv.ActualHeight / (ListViewItemLineHeight + 8)));
                }
            }));

        public DelegateCommand CreateCommand =>
            createCommand ?? (createCommand = new DelegateCommand(() =>
            {
                var dialogParam = new DialogParameters { { nameof(FileSystemInfo), new DirectoryInfo(CurrentDirectory.FullName) } };
                dialogService.ShowDialog(nameof(SelectionDialog), dialogParam, (IDialogResult dialogResult) => { });
                CurrentDirectory = new DirectoryInfo(CurrentDirectory.FullName);
            }));

        public DelegateCommand MarkCommand =>
            markCommand ?? (markCommand = new DelegateCommand(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.Marked = !SelectedItem.Marked;
                }
            }));

        public DelegateCommand<ListView> MarkAndDownCommand =>
            markAndDownCommand ?? (markAndDownCommand = new DelegateCommand<ListView>((param) =>
            {
                if (SelectedItem != null)
                {
                    if (ExecuteCounter == 0)
                    {
                        SelectedItem.Marked = !SelectedItem.Marked;
                        MoveCursor(param, 1);
                    }
                    else
                    {
                        for (var i = 0; i < ExecuteCounter; i++)
                        {
                            SelectedItem.Marked = !SelectedItem.Marked;
                            MoveCursor(param, 1);
                        }

                        ExecuteCounter = 0;
                    }
                }
            }));

        public DelegateCommand<string> NumberInputCommand => new DelegateCommand<string>((counter) =>
        {
            // 型が string なのは、例えば 1, 2 と入力を行ったとき、合わせて入力値が 12 になるようにするため
            if (ExecuteCounter < 10000)
            {
                ExecuteCounter = int.Parse(ExecuteCounter.ToString() + counter);
            }
        });

        public DelegateCommand ClearInputNumberCommand => new DelegateCommand(() => { ExecuteCounter = 0; });

        private void MoveCursor(ListView lv, int amount)
        {
            if (lv.SelectedIndex + amount < 0)
            {
                lv.SelectedIndex = 0;
            }
            else if (lv.SelectedIndex + amount > lv.Items.Count)
            {
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            else
            {
                lv.SelectedIndex += amount;
            }

            lv.ScrollIntoView(lv.Items[lv.SelectedIndex]);

            var item = lv.ItemContainerGenerator.ContainerFromIndex(lv.SelectedIndex) as ListViewItem;
            item.Focus();
            Keyboard.Focus(item);
        }

        private ObservableCollection<ExtendFileInfo> GetFileList(string path, OwnerListViewLocation destLocation)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);
            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            var files = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));

            var bothList = directories.Concat(files).ToList();

            bothList.ToList().ForEach(f => f.OwnerListViewLocation = destLocation);
            Enumerable.Range(0, bothList.Count).ToList().ForEach(n => bothList[n].Index = n + 1);

            return new ObservableCollection<ExtendFileInfo>(bothList);
        }
    }
}