using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Filer.Models
{
    public class FileContainer : BindableBase
    {
        private ObservableCollection<ExtendFileInfo> files = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;
        private int selectedIndex;

        public ObservableCollection<ExtendFileInfo> Files
        {
            get => files;
            set
            {
                if (value != null && value.Count > 0)
                {
                    SelectedIndex = 0;
                }

                SetProperty(ref files, value);
            }
        }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public int SelectedIndex
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }
    }
}