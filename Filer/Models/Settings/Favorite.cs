using Prism.Mvvm;

namespace Filer.Models.Settings
{
    public class Favorite : BindableBase
    {
        private string key;
        private string path;
        private string name;
        private bool isMatch;

        public string Key { get => key; set => SetProperty(ref key, value); }

        public string Path { get => path; set => SetProperty(ref path, value); }

        public string Name { get => name; set => SetProperty(ref name, value); }

        public bool IsMatch { get => isMatch; set => SetProperty(ref isMatch, value); }
    }
}