namespace Filer.Models
{
    using System.IO;
    using Prism.Mvvm;

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
    }
}
