using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Prism.Mvvm;

namespace Filer.Models
{
    public class Folder : BindableBase
    {
        private DirectoryInfo currentDirectory;
        private string fullName = string.Empty;
        private string name = string.Empty;
        private ObservableCollection<ExtendFileInfo> files = new ObservableCollection<ExtendFileInfo>();
        private bool selected;
        private SortStatus sortStatus = new SortStatus();

        public ObservableCollection<ExtendFileInfo> Files
        {
            get => files;
            private set
            {
                FileContainer.Files = value;
                SetProperty(ref files, value);
            }
        }

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

                Files = GetFileList(value.FullName);
                Name = value.Name;
                FullName = value.FullName;

                using (var dbContext = new DatabaseContext())
                {
                    dbContext.Add(value.FullName);
                }

                SetProperty(ref currentDirectory, value);

                if (oldDirectory == null)
                {
                    return;
                }

                // 移動先のディレクトリに移動元ディレクトリがあるならば(ディレクトリを一段登った場合等) そのディレクトリを選択する。
                var oldDirectoryParentPath = oldDirectory.Parent != null ? oldDirectory.Parent.FullName : string.Empty;
                if (oldDirectoryParentPath == value.FullName)
                {
                    FileContainer.SelectedItem =
                        Files.FirstOrDefault(d => d.FileSystemInfo.FullName == oldDirectory.FullName);
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

        public string Name { get => name; private set => SetProperty(ref name, value); }

        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        public bool Selected { get => selected; set => SetProperty(ref selected, value); }

        public OwnerListViewLocation OwnerListViewLocation { private get; set; }

        public Logger Logger { private get; set; }

        public FileContainer FileContainer { get; set; } = new FileContainer();

        public SortStatus SortStatus { get => sortStatus; set => SetProperty(ref sortStatus, value); }

        private ObservableCollection<ExtendFileInfo> GetFileList(string path)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);

            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            directories = Sort(directories);

            var fs = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));
            fs = Sort(fs);

            var bothList = directories.Concat(fs).ToList();

            Enumerable.Range(0, bothList.Count).ToList().ForEach(n => bothList[n].Index = n + 1);

            return new ObservableCollection<ExtendFileInfo>(bothList);
        }

        private IEnumerable<ExtendFileInfo> Sort(IEnumerable<ExtendFileInfo> fileList)
        {
            if (SortStatus == null)
            {
                return fileList;
            }

            IEnumerable<ExtendFileInfo> fs;
            switch (SortStatus.Key)
            {
                case SortStatus.SortKey.Name:
                    fs = fileList.OrderBy(f => f.Name);
                    break;
                case SortStatus.SortKey.Updated:
                    fs = fileList.OrderBy(f => f.UpdateTime);
                    break;
                case SortStatus.SortKey.Created:
                    fs = fileList.OrderBy(f => f.CreationTime);
                    break;
                case SortStatus.SortKey.Extension:
                    fs = fileList.OrderBy(f => f.Extension);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (SortStatus.Reverse)
            {
                fs = fs.Reverse();
            }

            return fs;
        }
    }
}