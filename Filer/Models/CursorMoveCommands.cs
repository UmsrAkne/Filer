using Prism.Commands;
using Prism.Mvvm;

namespace Filer.Models
{
    public class CursorMoveCommands : BindableBase
    {
        private DelegateCommand cursorUpCommand;
        private DelegateCommand cursorDownCommand;
        private DelegateCommand jumpToFirstCommand;
        private DelegateCommand jumpToLastCommand;
        private DelegateCommand pageDownCommand;
        private DelegateCommand pageUpCommand;

        private int executeCounter;
        private double listViewActualHeight;

        public FileContainer FileContainer { private get; set; }

        public double ListViewItemLineHeight { get; set; } = 15.0;

        public double ListViewActualHeight
        {
            get => listViewActualHeight;
            set => SetProperty(ref listViewActualHeight, value);
        }

        public DelegateCommand<string> NumberInputCommand => new DelegateCommand<string>((counter) =>
            {
                // 型が string なのは、例えば 1, 2 と入力を行ったとき、合わせて入力値が 12 になるようにするため
                if (ExecuteCounter < 10000)
                {
                    ExecuteCounter = int.Parse(ExecuteCounter + counter);
                }
            });

        public DelegateCommand PageUpCommand =>
            pageUpCommand ?? (pageUpCommand = new DelegateCommand(() =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                var itemCountPerPage = (int)(ListViewActualHeight / (ListViewItemLineHeight + 8));
                FileContainer.UpCursor(amount * itemCountPerPage);
            }));

        public DelegateCommand PageDownCommand =>
            pageDownCommand ?? (pageDownCommand = new DelegateCommand(() =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                var itemCountPerPage = (int)(ListViewActualHeight / (ListViewItemLineHeight + 8));
                FileContainer.DownCursor(amount * itemCountPerPage);
            }));

        public DelegateCommand JumpToLastCommand =>
            jumpToLastCommand ?? (jumpToLastCommand = new DelegateCommand(() =>
            {
                // FileContainer.JumpToLast();
                //
                // // 最後の行までジャンプした直後に ListViewItem が範囲選択されるので、選択状態をリセットしている。
                // var item = lv.SelectedItem;
                // var currentIndex = lv.SelectedIndex == -1 ? 0 : lv.SelectedIndex;
                // FileList.Skip(currentIndex).ToList().ForEach(f => f.IsSelected = false);
                // SelectedFolder.FileContainer.SelectedItem = item as ExtendFileInfo;
            }));

        public DelegateCommand JumpToFirstCommand =>
            jumpToFirstCommand ?? (jumpToFirstCommand = new DelegateCommand(() =>
            {
                FileContainer.JumpToHead();
            }));

        public DelegateCommand CursorDownCommand =>
            cursorDownCommand ?? (cursorDownCommand = new DelegateCommand(() =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                FileContainer.DownCursor(amount);
            }));

        public DelegateCommand CursorUpCommand =>
            cursorUpCommand ?? (cursorUpCommand = new DelegateCommand(() =>
            {
                var amount = ExecuteCounter != 0 ? ExecuteCounter : 1;
                ExecuteCounter = 0;
                FileContainer.UpCursor(amount);
            }));

        private int ExecuteCounter { get => executeCounter; set => SetProperty(ref executeCounter, value); }
    }
}