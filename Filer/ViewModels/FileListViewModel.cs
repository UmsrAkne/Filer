using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Filer.Models;
using Filer.Models.Settings;
using Filer.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Filer.ViewModels
{
    public class FileListViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private bool isFocused;
        private string commandText = string.Empty;
        private int executeCounter;
        private DelegateCommand<string> openPathCommand;
        private DelegateCommand<ListView> openFileCommand;
        private DelegateCommand openWithAppCommand;
        private DelegateCommand<ListView> cursorDownCommand;
        private DelegateCommand<ListView> cursorUpCommand;
        private DelegateCommand jumpToFirstCommand;
        private DelegateCommand<ListView> jumpToLastCommand;
        private DelegateCommand directoryUpCommand;
        private DelegateCommand<ListView> pageUpCommand;
        private DelegateCommand<ListView> pageDownCommand;
        private DelegateCommand createCommand;
        private DelegateCommand markCommand;
        private DelegateCommand<ListView> markAndDownCommand;
        private DelegateCommand<TextBox> focusCommandTextBoxCommand;
        private DelegateCommand<TextBox> startPartialMatchSearchCommand;
        private DelegateCommand searchFileCommand;
        private DelegateCommand addTabCommand;
        private DelegateCommand closeTabCommand;
        private DelegateCommand<object> changeTabCommand;
        private DelegateCommand<object> toggleTextInputCommand;
        private DelegateCommand selectionModeCommand;

        private DelegateCommand openBookmarkAdditionPageCommand;
        private DelegateCommand openBookmarkJumpPageCommand;

        private ExtendFileInfo selectedItem;
        private ObservableCollection<Folder> folders;
        private Folder selectedFolder;
        private int selectedFolderIndex;

        public FileListViewModel(IDialogService dialogService, OwnerListViewLocation location, Logger lg)
        {
            this.dialogService = dialogService;
            Logger = lg;
            OwnerListViewLocation = location;

            var defaultFolder = new Folder() { Logger = Logger, OwnerListViewLocation = location };
            Folders = new ObservableCollection<Folder>() { defaultFolder };
            SelectedFolder = defaultFolder;
        }

        public OwnerListViewLocation OwnerListViewLocation { get; set; }

        public ExtendFileInfo SelectedItem
        {
            get => SelectedFolder.FileContainer.SelectedItem;
            set => SelectedFolder.FileContainer.SelectedItem = value;
        }

        public bool IsFocused { get => isFocused; set => SetProperty(ref isFocused, value); }

        public string CommandText { get => commandText; set => SetProperty(ref commandText, value); }

        public ObservableCollection<ExtendFileInfo> FileList => SelectedFolder.Files;

        public ObservableCollection<Folder> Folders { get => folders; private set => SetProperty(ref folders, value); }

        public Folder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                if (selectedFolder != null)
                {
                    selectedFolder.Selected = false;
                }

                if (value != null)
                {
                    value.Selected = true;
                }

                SetProperty(ref selectedFolder, value);
            }
        }

        public bool TextInputting { get; private set; }

        public ListView ListView { private get; set; }

        public DirectoryInfo CurrentDirectory
        {
            get => SelectedFolder.CurrentDirectory;
            set => SelectedFolder.CurrentDirectory = value;
        }

        public DelegateCommand<string> OpenPathCommand =>
            openPathCommand ?? (openPathCommand = new DelegateCommand<string>((locationString) =>
            {
                CurrentDirectory = new DirectoryInfo(SelectedFolder.FullName);
            }));

        public DelegateCommand<ListView> OpenFileCommand =>
            openFileCommand ?? (openFileCommand = new DelegateCommand<ListView>((lv) =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsDirectory)
                    {
                        CurrentDirectory = SelectedItem.FileSystemInfo as DirectoryInfo;

                        if (lv.Items.Count > 0)
                        {
                            SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        Process.Start(SelectedItem.FileSystemInfo.FullName);
                    }
                }
            }));

        public DelegateCommand OpenWithAppCommand =>
            openWithAppCommand ?? (openWithAppCommand = new DelegateCommand(() =>
            {
                List<ExtendFileInfo> targets = new List<ExtendFileInfo>();

                // マークされているファイルがある場合はそれを優先する
                if (FileList.Any(f => f.Marked))
                {
                    targets = FileList.Where(f => f.Marked).ToList();
                }
                else if (SelectedItem != null)
                {
                    // マークがされていない場合は、現在カーソルが当たっているファイルを対象とする
                    targets = new List<ExtendFileInfo>() { SelectedItem };
                }

                if (targets.Count != 0)
                {
                    var param = new DialogParameters { { "OpenFileCount", targets.Count() } };
                    dialogService.ShowDialog(nameof(OpenWithAppPage), param, dialogResult =>
                    {
                        if (dialogResult.Parameters.ContainsKey(nameof(FileInfo)))
                        {
                            var executeFilePath = dialogResult.Parameters.GetValue<FileInfo>(nameof(FileInfo));
                            foreach (var f in targets)
                            {
                                Process.Start(executeFilePath.FullName, f.FileSystemInfo.FullName);
                            }
                        }
                    });
                }
            }));

        public DelegateCommand OpenBookmarkAdditionPageCommand =>
            openBookmarkAdditionPageCommand ?? (openBookmarkAdditionPageCommand = new DelegateCommand(() =>
            {
                var param = new DialogParameters
                {
                    { nameof(BookmarkPageViewModel.Mode), BookmarkPageViewModel.Mode.AdditionMode },
                    { nameof(ExtendFileInfo), new ExtendFileInfo(SelectedItem.FileSystemInfo.FullName) },
                };

                dialogService.ShowDialog(nameof(BookmarkPage), param, result => { });
            }));

        public DelegateCommand OpenBookmarkJumpPageCommand =>
            openBookmarkJumpPageCommand ?? (openBookmarkJumpPageCommand = new DelegateCommand(() =>
            {
                var param = new DialogParameters
                    { { nameof(BookmarkPageViewModel.Mode), BookmarkPageViewModel.Mode.JumpMode } };

                dialogService.ShowDialog(nameof(BookmarkPage), param, result =>
                {
                    if (result.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    var exFileInfo = new ExtendFileInfo(result.Parameters.GetValue<Favorite>(nameof(Favorite)).Path);
                    if (exFileInfo.IsDirectory)
                    {
                        CurrentDirectory = new DirectoryInfo(exFileInfo.FileSystemInfo.FullName).Parent;
                    }
                    else
                    {
                        var fileInfo = (FileInfo)exFileInfo.FileSystemInfo;
                        if (fileInfo.DirectoryName != null)
                        {
                            CurrentDirectory = new DirectoryInfo(fileInfo.DirectoryName);
                        }
                    }

                    SelectedItem = FileList.FirstOrDefault(f => f.Name == exFileInfo.Name);
                });
            }));

        public DelegateCommand<ListView> CursorDownCommand =>
            cursorDownCommand ?? (cursorDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                SelectedFolder.FileContainer.DownCursor(amount);
                FocusToListViewItem();
            }));

        public DelegateCommand<ListView> CursorUpCommand =>
            cursorUpCommand ?? (cursorUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                SelectedFolder.FileContainer.UpCursor(amount);
                FocusToListViewItem();
            }));

        public DelegateCommand<ListView> JumpToLastCommand =>
            jumpToLastCommand ?? (jumpToLastCommand = new DelegateCommand<ListView>((lv) =>
            {
                SelectedFolder.FileContainer.JumpToLast();
                FocusToListViewItem();

                // 最後の行までジャンプした直後に ListViewItem が範囲選択されるので、選択状態をリセットしている。
                var item = lv.SelectedItem;
                var currentIndex = lv.SelectedIndex == -1 ? 0 : lv.SelectedIndex;
                FileList.Skip(currentIndex).ToList().ForEach(f => f.IsSelected = false);
                SelectedFolder.FileContainer.SelectedItem = item as ExtendFileInfo;
            }));

        public DelegateCommand JumpToFirstCommand =>
            jumpToFirstCommand ?? (jumpToFirstCommand = new DelegateCommand(() =>
            {
                SelectedFolder.FileContainer.JumpToHead();
                FocusToListViewItem();

                if (SelectedIndex != -1)
                {
                    SelectedIndex = 0;
                }
            }));

        public DelegateCommand DirectoryUpCommand =>
            directoryUpCommand ?? (directoryUpCommand = new DelegateCommand(() =>
            {
                var parentDirectoryInfo = new DirectoryInfo(CurrentDirectory.FullName).Parent;

                if (parentDirectoryInfo != null)
                {
                    CurrentDirectory = parentDirectoryInfo;
                }
            }));

        public DelegateCommand<ListView> PageUpCommand =>
            pageUpCommand ?? (pageUpCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                var itemCountPerPage = (int)(lv.ActualHeight / (ListViewItemLineHeight + 8));
                SelectedFolder.FileContainer.UpCursor(amount * itemCountPerPage);
                FocusToListViewItem();
            }));

        public DelegateCommand<ListView> PageDownCommand =>
            pageDownCommand ?? (pageDownCommand = new DelegateCommand<ListView>((lv) =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                var itemCountPerPage = (int)(lv.ActualHeight / (ListViewItemLineHeight + 8));
                SelectedFolder.FileContainer.DownCursor(amount * itemCountPerPage);
                FocusToListViewItem();
            }));

        public DelegateCommand CreateCommand =>
            createCommand ?? (createCommand = new DelegateCommand(() =>
            {
                var dialogParam = new DialogParameters { { nameof(FileSystemInfo), new DirectoryInfo(CurrentDirectory.FullName) } };
                dialogService.ShowDialog(nameof(SelectionDialog), dialogParam, dialogResult => { });
                CurrentDirectory = new DirectoryInfo(CurrentDirectory.FullName);
            }));

        public DelegateCommand MarkCommand =>
            markCommand ?? (markCommand = new DelegateCommand(() =>
            {
                if (SelectedFolder.FileContainer.SelectionMode)
                {
                    foreach (var f in FileList.Where(f => f.IsSelectionModeSelected))
                    {
                        f.Marked = !f.Marked;
                    }

                    SelectedFolder.FileContainer.SelectionMode = false;
                    return;
                }

                if (SelectedItem != null)
                {
                    SelectedItem.Marked = !SelectedItem.Marked;
                }
            }));

        public DelegateCommand<ListView> MarkAndDownCommand =>
            markAndDownCommand ?? (markAndDownCommand = new DelegateCommand<ListView>((param) =>
            {
                if (SelectedItem != null)
                {
                    if (ExecuteCounter == 0)
                    {
                        SelectedItem.Marked = !SelectedItem.Marked;
                        MoveCursor(param, 1);
                    }
                    else
                    {
                        for (var i = 0; i < ExecuteCounter; i++)
                        {
                            SelectedItem.Marked = !SelectedItem.Marked;
                            MoveCursor(param, 1);
                        }

                        ExecuteCounter = 0;
                    }
                }
            }));

        public DelegateCommand<object> ToggleTextInputCommand =>
            toggleTextInputCommand ?? (toggleTextInputCommand = new DelegateCommand<object>(b =>
            {
                TextInputting = (bool)b; // パラメーターは Nullable しか受け付けないので　object で受け取る
            }));

        public DelegateCommand<TextBox> FocusCommandTextBoxCommand =>
            focusCommandTextBoxCommand ?? (focusCommandTextBoxCommand = new DelegateCommand<TextBox>(t =>
            {
                t.Focus();
                t.Text = "^.*";
                t.SelectionStart = 1;
            }));

        public DelegateCommand<TextBox> StartPartialMatchSearchCommand =>
            startPartialMatchSearchCommand ?? (startPartialMatchSearchCommand = new DelegateCommand<TextBox>(t =>
            {
                t.Focus();
                t.Text = string.Empty;
            }));

        public DelegateCommand SearchFileCommand =>
            searchFileCommand ?? (searchFileCommand = new DelegateCommand(() =>
            {
                // 現在選択中の要素の次の要素から検索を開始する
                var matched = FileList.Skip(SelectedIndex + 1).FirstOrDefault(f => Regex.IsMatch(f.Name, CommandText));

                if (matched != null)
                {
                    SelectedIndex = matched.Index - 1;
                    FocusToListViewItem();
                }
                else
                {
                    Logger.FileNotFound(CommandText);
                }
            }));

        public DelegateCommand<string> NumberInputCommand => new DelegateCommand<string>((counter) =>
        {
            // 型が string なのは、例えば 1, 2 と入力を行ったとき、合わせて入力値が 12 になるようにするため
            if (ExecuteCounter < 10000)
            {
                ExecuteCounter = int.Parse(ExecuteCounter.ToString() + counter);
            }
        });

        public DelegateCommand ClearInputNumberCommand => new DelegateCommand(() =>
        {
            FocusToListViewItem();
            ExecuteCounter = 0;
            SelectedFolder.FileContainer.SelectionMode = false;
        });

        public DelegateCommand AddTabCommand =>
            addTabCommand ?? (addTabCommand = new DelegateCommand(() =>
            {
                var folder = new Folder()
                {
                    Logger = Logger,
                    OwnerListViewLocation = OwnerListViewLocation,
                    CurrentDirectory = SelectedFolder.CurrentDirectory,
                };

                Folders.Add(folder);
            }));

        public DelegateCommand<object> ChangeTabCommand =>
            changeTabCommand ?? (changeTabCommand = new DelegateCommand<object>((amount) =>
            {
                if (SelectedFolderIndex < 0 || Folders.Count <= 1)
                {
                    return;
                }

                var count = int.Parse((string)amount);

                if (SelectedFolderIndex + count < 0)
                {
                    SelectedFolderIndex = Folders.Count - 1;
                }
                else if (SelectedFolderIndex + count >= Folders.Count)
                {
                    SelectedFolderIndex = 0;
                }
                else
                {
                    SelectedFolderIndex += count;
                }

                FocusToListViewItem();
            }));

        public DelegateCommand CloseTabCommand =>
            closeTabCommand ?? (closeTabCommand = new DelegateCommand(() =>
            {
                if (Folders.Count < 2)
                {
                    return; // タブを一つしか開いていない状態では発動不要
                }

                var currentIndex = SelectedFolderIndex;
                Folders.Remove(SelectedFolder);

                var newIndex = currentIndex == Folders.Count ? currentIndex - 1 : currentIndex;
                SelectedFolderIndex = newIndex;
                SelectedFolder = Folders[newIndex];
            }));

        public DelegateCommand SelectionModeCommand =>
            selectionModeCommand ?? (selectionModeCommand = new DelegateCommand(() =>
            {
                SelectedFolder.FileContainer.SelectionMode = true;
            }));

        public int SelectedFolderIndex { get => selectedFolderIndex; set => SetProperty(ref selectedFolderIndex, value); }

        private int ExecuteCounter { get => executeCounter; set => SetProperty(ref executeCounter, value); }

        private double ListViewItemLineHeight => 15.0;

        private Logger Logger { get; }

        private int SelectedIndex
        {
            get => SelectedFolder.FileContainer.SelectedIndex;
            set => SelectedFolder.FileContainer.SelectedIndex = value;
        }

        private void FocusToListViewItem()
        {
            if (ListView == null)
            {
                return;
            }

            if (FileList.Count == 0)
            {
                // このブロックに入った場合は ListViewItem にフォーカスすることはできない
                // 代わりに ListView 自体にフォーカスする
                ListView.Focus();
                Keyboard.Focus(ListView);
                return;
            }

            SelectedIndex = SelectedIndex == -1 ? 0 : SelectedIndex;

            if (ListView?.ItemContainerGenerator.ContainerFromIndex(ListView.SelectedIndex) is ListViewItem item)
            {
                item.Focus();
                Keyboard.Focus(item);
            }
        }

        private void MoveCursor(ListView lv, int amount)
        {
            if (!FileList.Any())
            {
                return;
            }

            if (lv.SelectedIndex + amount < 0)
            {
                lv.SelectedIndex = 0;
            }
            else if (lv.SelectedIndex + amount > lv.Items.Count)
            {
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            else
            {
                lv.SelectedIndex += amount;
            }

            lv.ScrollIntoView(lv.Items[lv.SelectedIndex]);

            if (lv.ItemContainerGenerator.ContainerFromIndex(lv.SelectedIndex) is ListViewItem item)
            {
                item.Focus();
                Keyboard.Focus(item);
            }
        }
    }
}