namespace Filer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        private ObservableCollection<ExtendFileInfo> leftFileList = new ObservableCollection<ExtendFileInfo>();
        private ObservableCollection<ExtendFileInfo> rightFileList = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;

        private string leftPathBarText;
        private string rightPathBarText;

        private int leftListViewSelectedIndex;
        private int rightListViewSelectedIndex;

        //// DelegateCommand *******************************************************

        private DelegateCommand openFileCommand;
        private DelegateCommand<ListView> focusToListViewCommand;

        public MainWindowViewModel()
        {
            var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            LeftFileList = GetFileList(defaultPath, OwnerListViewLocation.Left);
            RightFileList = GetFileList(defaultPath, OwnerListViewLocation.Right);

            LeftFileListViewModel.FileList = GetFileList(defaultPath, OwnerListViewLocation.Left);
            RightFileListViewModel.FileList = GetFileList(defaultPath, OwnerListViewLocation.Right);
            LeftFileListViewModel.PathBarText = defaultPath;
            RightFileListViewModel.PathBarText = defaultPath;
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public FileListViewModel LeftFileListViewModel { get; } = new FileListViewModel() { OwnerListViewLocation = OwnerListViewLocation.Left };

        public FileListViewModel RightFileListViewModel { get; } = new FileListViewModel() { OwnerListViewLocation = OwnerListViewLocation.Right };

        public ObservableCollection<ExtendFileInfo> LeftFileList { get => leftFileList; set => SetProperty(ref leftFileList, value); }

        public ObservableCollection<ExtendFileInfo> RightFileList { get => rightFileList; set => SetProperty(ref rightFileList, value); }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string LeftPathBarText { get => leftPathBarText; set => SetProperty(ref leftPathBarText, value); }

        public string RightPathBarText { get => rightPathBarText; set => SetProperty(ref rightPathBarText, value); }

        public int LeftListViewSelectedIndex { get => leftListViewSelectedIndex; set => SetProperty(ref leftListViewSelectedIndex, value); }

        public int RightListViewSelectedIndex { get => rightListViewSelectedIndex; set => SetProperty(ref rightListViewSelectedIndex, value); }

        //// DelegateCommand *******************************************************

        public DelegateCommand OpenFileCommand
        {
            get => openFileCommand ?? (openFileCommand = new DelegateCommand(() =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsDirectory)
                    {
                        if (SelectedItem.OwnerListViewLocation == OwnerListViewLocation.Left)
                        {
                            LeftFileList = GetFileList(SelectedItem.FileSystemInfo.FullName, SelectedItem.OwnerListViewLocation);
                            LeftListViewSelectedIndex = 0;
                        }
                        else
                        {
                            RightFileList = GetFileList(SelectedItem.FileSystemInfo.FullName, SelectedItem.OwnerListViewLocation);
                            RightListViewSelectedIndex = 0;
                        }
                    }
                    else
                    {
                        Process.Start(SelectedItem.FileSystemInfo.FullName);
                    }
                }
            }));
        }

        public DelegateCommand<ListView> FocusToListViewCommand
        {
            get => focusToListViewCommand ?? (focusToListViewCommand = new DelegateCommand<ListView>((lv) =>
            {
                var destIndex = lv.SelectedIndex < 0 ? 0 : lv.SelectedIndex;
                var item = lv.ItemContainerGenerator.ContainerFromIndex(destIndex) as ListViewItem;
                Keyboard.Focus(item);
            }));
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

        private ListView GetFocusingListView()
        {
            if (Keyboard.FocusedElement == null || !(Keyboard.FocusedElement is ListViewItem))
            {
                return null;
            }

            var obj = (DependencyObject)Keyboard.FocusedElement;

            while (!(obj is ListView))
            {
                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);

                if (obj == null)
                {
                    break;
                }
            }

            return (obj != null) ? (ListView)obj : null;
        }
    }
}
