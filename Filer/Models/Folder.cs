using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Prism.Mvvm;

namespace Filer.Models
{
    public class Folder : BindableBase
    {
        private DirectoryInfo currentDirectory;
        private ObservableCollection<ExtendFileInfo> files;
        private string fullName = string.Empty;
        private string name = string.Empty;
        private bool selected;

        public ObservableCollection<ExtendFileInfo> Files
        {
            get => files;
            set => SetProperty(ref files, value);
        }

        public DirectoryInfo CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                currentDirectory = value;

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

                Files = GetFileList(value.FullName);
                Name = value.Name;
                FullName = value.FullName;

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

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        public bool Selected { get => selected; set => SetProperty(ref selected, value); }

        public OwnerListViewLocation OwnerListViewLocation { private get; set; }

        public Logger Logger { private get; set; }

        private ObservableCollection<ExtendFileInfo> GetFileList(string path)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);
            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            var fs = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));

            var bothList = directories.Concat(fs).ToList();

            Enumerable.Range(0, bothList.Count).ToList().ForEach(n => bothList[n].Index = n + 1);

            return new ObservableCollection<ExtendFileInfo>(bothList);
        }
    }
}