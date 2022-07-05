namespace Filer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class FileListViewModel : BindableBase
    {
        private string pathBarText;
        private int selectedIndex;
        private ObservableCollection<ExtendFileInfo> fileList;
        private DelegateCommand<string> openPathCommand;

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
