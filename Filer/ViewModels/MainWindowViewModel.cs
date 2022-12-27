using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private IDialogService dialogService;

        private ExtendFileInfo selectedItem;

        //// DelegateCommand *******************************************************

        private DelegateCommand<ListView> focusToListViewCommand;

        public MainWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            LeftFileListViewModel = new FileListViewModel(dialogService)
            {
                OwnerListViewLocation = OwnerListViewLocation.Left,
                Logger = Logger,
                CurrentDirectory = new DirectoryInfo(defaultPath),
            };

            RightFileListViewModel = new FileListViewModel(dialogService)
            {
                OwnerListViewLocation = OwnerListViewLocation.Right,
                Logger = Logger,
                CurrentDirectory = new DirectoryInfo(defaultPath),
            };
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public FileListViewModel LeftFileListViewModel { get; }

        public FileListViewModel RightFileListViewModel { get; }

        public ExtendFileInfo SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public Logger Logger { get; } = new Logger();

        //// DelegateCommand *******************************************************

        public DelegateCommand<ListView> FocusToListViewCommand =>
            focusToListViewCommand ?? (focusToListViewCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (lv.Items.Count == 0)
                {
                    Keyboard.Focus(lv);
                }
                else
                {
                    var destIndex = lv.SelectedIndex < 0 ? 0 : lv.SelectedIndex;
                    lv.SelectedIndex = destIndex;
                    var item = lv.ItemContainerGenerator.ContainerFromIndex(destIndex) as ListViewItem;
                    Keyboard.Focus(item);
                }
            }));

        private ObservableCollection<ExtendFileInfo> GetFileList(string path, OwnerListViewLocation destLocation)
        {
            var defaultDirectoryInfo = new DirectoryInfo(path);
            var directories = defaultDirectoryInfo.GetDirectories().Select(d => new ExtendFileInfo(d.FullName));
            var files = defaultDirectoryInfo.GetFiles().Select(f => new ExtendFileInfo(f.FullName));

            var bothList = directories.Concat(files).ToList();

            bothList.ToList().ForEach(f => f.OwnerListViewLocation = destLocation);
            Enumerable.Range(0, bothList.Count).ToList().ForEach(n => bothList[n].Index = n + 1);

            return new ObservableCollection<ExtendFileInfo>(bothList);
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

            return (ListView)obj;
        }
    }
}