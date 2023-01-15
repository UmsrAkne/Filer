using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Filer.Models
{
    public class FileContainer : BindableBase
    {
        private ObservableCollection<ExtendFileInfo> files = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;
        private int selectedIndex = -1;

        public ObservableCollection<ExtendFileInfo> Files
        {
            get => files;
            set
            {
                if (value != null && value.Count > 0)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex = -1;
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

        public bool CanMoveCursor => Files != null && Files.Count != 0;

        public void DownCursor(int count)
        {
            if (!CanMoveCursor)
            {
                return;
            }

            if (SelectedIndex + count >= Files.Count - 1)
            {
                SelectedIndex = Files.Count - 1;
            }
            else
            {
                SelectedIndex += count;
            }
        }
    }
}