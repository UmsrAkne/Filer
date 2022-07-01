namespace Filer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Filer.Models;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        private ObservableCollection<ExtendFileInfo> leftFileList = new ObservableCollection<ExtendFileInfo>();
        private ObservableCollection<ExtendFileInfo> rightFileList = new ObservableCollection<ExtendFileInfo>();

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
