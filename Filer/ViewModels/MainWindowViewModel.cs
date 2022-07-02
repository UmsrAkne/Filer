namespace Filer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        private ObservableCollection<ExtendFileInfo> leftFileList = new ObservableCollection<ExtendFileInfo>();
        private ObservableCollection<ExtendFileInfo> rightFileList = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;

        //// DelegateCommand *******************************************************

        private DelegateCommand openFileCommand;

        public MainWindowViewModel()
        {
            LeftFileList = GetFileList(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            RightFileList = GetFileList(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<ExtendFileInfo> LeftFileList { get => leftFileList; set => SetProperty(ref leftFileList, value); }

        public ObservableCollection<ExtendFileInfo> RightFileList { get => rightFileList; set => SetProperty(ref rightFileList, value); }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        //// DelegateCommand *******************************************************

        public DelegateCommand OpenFileCommand
        {
            get => openFileCommand ?? (openFileCommand = new DelegateCommand(() =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsDirectory)
                    {
                        LeftFileList = GetFileList(SelectedItem.FileSystemInfo.FullName);
                    }
                    else
                    {
                        Process.Start(SelectedItem.FileSystemInfo.FullName);
                    }
                }
            }));
        }

        private ObservableCollection<ExtendFileInfo> GetFileList(string path)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);
            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            var files = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));

            var bothList = directories.Concat(files);

            return new ObservableCollection<ExtendFileInfo>(directories.Concat(files));
        }
    }
}
