namespace Filer.ViewModels
{
    using System.Collections.ObjectModel;
    using Filer.Models;
    using Prism.Mvvm;

    public class FileListViewModel : BindableBase
    {
        private string pathBarText;
        private int selectedIndex;
        private ObservableCollection<ExtendFileInfo> fileList;

        private ExtendFileInfo selectedItem;

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string PathBarText { get => pathBarText; set => SetProperty(ref pathBarText, value); }

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public ObservableCollection<ExtendFileInfo> FileList { get => fileList; set => SetProperty(ref fileList, value); }
    }
}
