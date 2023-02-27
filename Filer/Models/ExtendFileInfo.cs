using System;

namespace Filer.Models
{
    using System.IO;
    using Prism.Mvvm;

    public class ExtendFileInfo : BindableBase
    {
        private bool isSelected;
        private int index;
        private bool marked;
        private bool isSelectionModeSelected;

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

        public DateTime CreationTime => FileSystemInfo.CreationTime;

        public DateTime UpdateTime => FileSystemInfo.LastWriteTime;

        public long FileSize => IsDirectory ? 0 : ((FileInfo)FileSystemInfo).Length;

        public bool IsSelectionModeSelected
        {
            get => isSelectionModeSelected;
            set => SetProperty(ref isSelectionModeSelected, value);
        }

        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        public int Index { get => index; set => SetProperty(ref index, value); }

        public bool Marked { get => marked; set => SetProperty(ref marked, value); }

        public OwnerListViewLocation OwnerListViewLocation { get; set; }

        public void Delete()
        {
            if (IsDirectory)
            {
                (FileSystemInfo as DirectoryInfo)?.Delete(true);
            }
            else
            {
                FileSystemInfo.Delete();
            }
        }

        public void Copy(string sourcePath, string destinationPath)
        {
            if (IsDirectory)
            {
                CopyDirectory(sourcePath, destinationPath);
            }
            else
            {
                File.Copy(sourcePath, destinationPath);
            }
        }

        public void Move(string sourcePath, string destinationPath)
        {
            if (IsDirectory)
            {
                Directory.Move(sourcePath, destinationPath);
            }
            else
            {
                File.Move(sourcePath, destinationPath);
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            var dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (var file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }
    }
}