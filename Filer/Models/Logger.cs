using System.IO;
using Prism.Mvvm;

namespace Filer.Models
{
    public class Logger : BindableBase
    {
        private string log = string.Empty;

        public string Log { get => log; set => SetProperty(ref log, value); }

        public void ChangeCurrentDirectoryLog(DirectoryInfo current, DirectoryInfo dest)
        {
            if (current != null && dest != null)
            {
                Log = $"カレントディレクトリを移動 \n  {current.FullName} --> {dest.FullName}\n\n" + Log;
            }
        }

        public void ReloadDirectory(DirectoryInfo current, OwnerListViewLocation ownerListViewLocation)
        {
            if (current != null)
            {
                LogToTop($"ディレクトリをリロード ({ownerListViewLocation}) {current.FullName}");
            }
        }

        public void FailAccess(DirectoryInfo current)
        {
            LogToTop($"ディレクトリへのアクセスに失敗 ({current.FullName})");
        }

        public void FailDelete(FileSystemInfo current)
        {
            LogToTop($"ファイルの削除に失敗 ({current.FullName})");
        }

        public void FileDeleted(FileSystemInfo current)
        {
            LogToTop($"ファイルを削除しました　({current.FullName})");
        }

        public void FileNotFound(string fileName)
        {
            LogToTop($"\"{fileName}\" は見つかりませんでした");
        }

        private void LogToTop(string msg)
        {
            Log = $"{msg}\n{Log}";
        }
    }
}