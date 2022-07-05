namespace Filer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class FileListViewModel : BindableBase
    {
        private string pathBarText;
        private int selectedIndex;
        private ObservableCollection<ExtendFileInfo> fileList;
        private DelegateCommand<string> openPathCommand;
        private DelegateCommand<ListView> openFileCommand;
        private DelegateCommand<ListView> cursorDownCommand;
        private DelegateCommand<ListView> cursorUpCommand;

        private ExtendFileInfo selectedItem;

        public OwnerListViewLocation OwnerListViewLocation { get; set; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string PathBarText { get => pathBarText; set => SetProperty(ref pathBarText, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public ObservableCollection<ExtendFileInfo> FileList { get => fileList; set => SetProperty(ref fileList, value); }

        public DelegateCommand<string> OpenPathCommand
        {
            get => openPathCommand ?? (openPathCommand = new DelegateCommand<string>((locationString) =>
            {
                FileList = GetFileList(PathBarText, OwnerListViewLocation);
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
                        FileList = GetFileList(SelectedItem.FileSystemInfo.FullName, OwnerListViewLocation);

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

        private void MoveCursor(ListView lv, int amount)
        {
            if (lv.SelectedIndex + amount < 0)
            {
                lv.SelectedIndex = 0;
            }
            else if (lv.SelectedIndex + amount > lv.Items.Count)
            {
                lv.SelectedItem = lv.Items.Count - 1;
            }
            else
            {
                lv.SelectedIndex += amount;
            }

            var item = lv.ItemContainerGenerator.ContainerFromIndex(lv.SelectedIndex) as ListViewItem;
            item.Focus();
            lv.ScrollIntoView(item);
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
