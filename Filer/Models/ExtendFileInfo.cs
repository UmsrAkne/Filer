namespace Filer.Models
{
    using System.IO;
    using Prism.Mvvm;

    public class ExtendFileInfo : BindableBase
    {
        private bool isSelected;
        private int index;
        private bool marked;

        public ExtendFileInfo(string path)
        {
            if (Directory.Exists(path))
            {
                FileSystemInfo = new DirectoryInfo(path);
                IsDirectory = true;
            }
            else
            {
                FileSystemInfo = new FileInfo(path);
            }
        }

        public FileSystemInfo FileSystemInfo { get; private set; }

        public string Name => FileSystemInfo.Name;

        public bool IsDirectory { get; }

        public string Extension => IsDirectory ? "DIR" : FileSystemInfo.Extension;

        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        public int Index { get => index; set => SetProperty(ref index, value); }

        public bool Marked { get => marked; set => SetProperty(ref marked, value); }
    }
}
