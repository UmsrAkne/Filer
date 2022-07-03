namespace Filer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Filer.Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        private ObservableCollection<ExtendFileInfo> leftFileList = new ObservableCollection<ExtendFileInfo>();
        private ObservableCollection<ExtendFileInfo> rightFileList = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;

        private string leftPathBarText;
        private string rightPathBarText;

        //// DelegateCommand *******************************************************

        private DelegateCommand openFileCommand;
        private DelegateCommand<ListView> focusToListViewCommand;
        private DelegateCommand cursorUpCommand;
        private DelegateCommand cursorDownCommand;

        public MainWindowViewModel()
        {
            var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            LeftFileList = GetFileList(defaultPath, OwnerListViewLocation.Left);
            RightFileList = GetFileList(defaultPath, OwnerListViewLocation.Right);
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<ExtendFileInfo> LeftFileList { get => leftFileList; set => SetProperty(ref leftFileList, value); }

        public ObservableCollection<ExtendFileInfo> RightFileList { get => rightFileList; set => SetProperty(ref rightFileList, value); }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string LeftPathBarText { get => leftPathBarText; set => SetProperty(ref leftPathBarText, value); }

        public string RightPathBarText { get => rightPathBarText; set => SetProperty(ref rightPathBarText, value); }

        //// DelegateCommand *******************************************************

        public DelegateCommand OpenFileCommand
        {
            get => openFileCommand ?? (openFileCommand = new DelegateCommand(() =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsDirectory)
                    {
                        LeftFileList = GetFileList(SelectedItem.FileSystemInfo.FullName, SelectedItem.OwnerListViewLocation);
                    }
                    else
                    {
                        Process.Start(SelectedItem.FileSystemInfo.FullName);
                    }
                }
            }));
        }

        public DelegateCommand<ListView> FocusToListViewCommand
        {
            get => focusToListViewCommand ?? (focusToListViewCommand = new DelegateCommand<ListView>((lv) =>
            {
                var destIndex = lv.SelectedIndex < 0 ? 0 : lv.SelectedIndex;
                var item = lv.ItemContainerGenerator.ContainerFromIndex(destIndex) as ListViewItem;
                Keyboard.Focus(item);
            }));
        }

        public DelegateCommand CursorDownCommand
        {
            get => cursorDownCommand ?? (cursorDownCommand = new DelegateCommand(() => MoveCursor(1)));
        }

        public DelegateCommand CursorUpCommand
        {
            get => cursorUpCommand ?? (cursorUpCommand = new DelegateCommand(() => MoveCursor(-1)));
        }

        private void MoveCursor(int amount)
        {
            if (Keyboard.FocusedElement != null && Keyboard.FocusedElement is ListViewItem)
            {
                ListView lv = GetFocusingListView();
                lv.SelectedIndex += amount;
                var item = lv.ItemContainerGenerator.ContainerFromIndex(lv.SelectedIndex) as ListViewItem;
                item.Focus();
                lv.ScrollIntoView(item);
            }
        }

        private ObservableCollection<ExtendFileInfo> GetFileList(string path, OwnerListViewLocation destLocation)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);
            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            var files = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));

            var bothList = directories.Concat(files);

            bothList.ToList().ForEach(f => f.OwnerListViewLocation = destLocation);

            return new ObservableCollection<ExtendFileInfo>(directories.Concat(files));
        }

        private ListView GetFocusingListView()
        {
            if (Keyboard.FocusedElement == null || !(Keyboard.FocusedElement is ListViewItem))
            {
                return null;
            }

            var obj = (DependencyObject)Keyboard.FocusedElement;

            while (!(obj is ListView))
            {
                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);

                if (obj == null)
                {
                    break;
                }
            }

            return (obj != null) ? (ListView)obj : null;
        }
    }
}
