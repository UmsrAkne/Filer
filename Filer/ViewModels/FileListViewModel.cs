namespace Filer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class FileListViewModel : BindableBase
    {
        private string pathBarText;
        private int selectedIndex;
        private ObservableCollection<ExtendFileInfo> fileList;
        private DirectoryInfo currentDirectory;
        private double listViewItemLineHeight = 15.0;
        private DelegateCommand<string> openPathCommand;
        private DelegateCommand<ListView> openFileCommand;
        private DelegateCommand<ListView> cursorDownCommand;
        private DelegateCommand<ListView> cursorUpCommand;
        private DelegateCommand<ListView> jumpToLastCommand;
        private DelegateCommand directoryUpCommand;
        private DelegateCommand<ListView> pageUpCommand;
        private DelegateCommand<ListView> pageDownCommand;

        private ExtendFileInfo selectedItem;

        public OwnerListViewLocation OwnerListViewLocation { get; set; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string PathBarText { get => pathBarText; set => SetProperty(ref pathBarText, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public ObservableCollection<ExtendFileInfo> FileList { get => fileList; set => SetProperty(ref fileList, value); }

        public double ListViewItemLineHeight { get => listViewItemLineHeight; set => SetProperty(ref listViewItemLineHeight, value); }

        public Logger Logger { private get; set; }

        public DirectoryInfo CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                FileList = GetFileList(value.FullName, OwnerListViewLocation);
                PathBarText = value.FullName;
                Logger.ChangeCurrentDirectoryLog(currentDirectory, value);
                currentDirectory = value;
            }
        }

        public DelegateCommand<string> OpenPathCommand
        {
            get => openPathCommand ?? (openPathCommand = new DelegateCommand<string>((locationString) =>
            {
                CurrentDirectory = new DirectoryInfo(PathBarText);
            }));
        }

        public DelegateCommand<ListView> OpenFileCommand
        {
            get => openFileCommand ?? (openFileCommand = new DelegateCommand<ListView>((lv) =>
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
        }

        public DelegateCommand<ListView> CursorDownCommand
        {
            get => cursorDownCommand ?? (cursorDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                MoveCursor(lv, 1);
            }));
        }

        public DelegateCommand<ListView> CursorUpCommand
        {
            get => cursorUpCommand ?? (cursorUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                MoveCursor(lv, -1);
            }));
        }

        public DelegateCommand<ListView> JumpToLastCommand
        {
            get => jumpToLastCommand ?? (jumpToLastCommand = new DelegateCommand<ListView>((lv) =>
            {
                var currentIndex = lv.SelectedIndex == -1 ? 0 : lv.SelectedIndex;
                MoveCursor(lv, FileList.Count() + 1);

                // 最後の行までジャンプした直後に ListViewItem が範囲選択されるので、選択状態をリセットしている。
                var item = lv.SelectedItem;
                FileList.Skip(currentIndex).ToList().ForEach(f => f.IsSelected = false);
                SelectedItem = item as ExtendFileInfo;
            }));
        }

        public DelegateCommand DirectoryUpCommand
        {
            get => directoryUpCommand ?? (directoryUpCommand = new DelegateCommand(() =>
            {
                var parentDirectoryInfo = new DirectoryInfo(CurrentDirectory.FullName).Parent;

                if (parentDirectoryInfo != null)
                {
                    CurrentDirectory = parentDirectoryInfo;
                }
            }));
        }

        public DelegateCommand<ListView> PageUpCommand
        {
            get => pageUpCommand ?? (pageUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                MoveCursor(lv, (int)(lv.ActualHeight / (ListViewItemLineHeight + 8)) * -1);
            }));
        }

        public DelegateCommand<ListView> PageDownCommand
        {
            get => pageDownCommand ?? (pageDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                MoveCursor(lv, (int)(lv.ActualHeight / (ListViewItemLineHeight + 8)));
            }));
        }

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
