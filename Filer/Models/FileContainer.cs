using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Prism.Mvvm;

namespace Filer.Models
{
    public class FileContainer : BindableBase
    {
        private ObservableCollection<ExtendFileInfo> files = new ObservableCollection<ExtendFileInfo>();
        private ExtendFileInfo selectedItem;
        private int selectedIndex = -1;
        private bool selectionMode;

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

        public bool SelectionMode
        {
            get => selectionMode;
            set
            {
                if (!CanMoveCursor)
                {
                    SetProperty(ref selectionMode, false);
                    return;
                }

                if (value)
                {
                    SelectionStartIndex = SelectedIndex;
                    Files[SelectedIndex].IsSelectionModeSelected = true;
                }

                if (selectionMode && !value)
                {
                    // !value でも SelectionMode でない場合は IsSelectionModeSelected の初期化処理は不要
                    SelectionStartIndex = -1;
                    Files.Where(f => f.IsSelectionModeSelected).ToList()
                        .ForEach(f => f.IsSelectionModeSelected = false);
                }

                SetProperty(ref selectionMode, value);
            }
        }

        private int SelectionStartIndex { get; set; }

        private bool CanMoveCursor => Files != null && Files.Count != 0;

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

            UpdateSelectionRange();
        }

        public void UpCursor(int count)
        {
            if (!CanMoveCursor)
            {
                return;
            }

            if (SelectedIndex - count < 0)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedIndex -= count;
            }

            UpdateSelectionRange();
        }

        public void JumpToHead()
        {
            if (!CanMoveCursor)
            {
                return;
            }

            SelectedIndex = 0;

            UpdateSelectionRange();
        }

        public void JumpToLast()
        {
            if (!CanMoveCursor)
            {
                return;
            }

            SelectedIndex = Files.Count - 1;

            UpdateSelectionRange();
        }

        public void JumpToNextFileName(string searchPattern, Logger logger)
        {
            if (!CanMoveCursor)
            {
                return;
            }

            // 現在選択中の要素の次の要素から検索を開始する
            var matched = Files.Skip(SelectedIndex + 1).FirstOrDefault(f => Regex.IsMatch(f.Name, searchPattern));

            if (matched != null)
            {
                SelectedIndex = Files.IndexOf(matched);
                UpdateSelectionRange();
            }
            else
            {
                logger.FileNotFound(searchPattern);
            }
        }

        private void UpdateSelectionRange()
        {
            if (!SelectionMode)
            {
                return;
            }

            foreach (var f in Files.Where(f => f.IsSelectionModeSelected))
            {
                f.IsSelectionModeSelected = false;
            }

            if (SelectionStartIndex == SelectedIndex)
            {
                Files[SelectionStartIndex].IsSelectionModeSelected = true;
                return;
            }

            var startIdx = Math.Min(SelectionStartIndex, SelectedIndex);
            var endIdx = Math.Max(SelectionStartIndex, SelectedIndex);

            foreach (var f in Files.Skip(startIdx).Take(endIdx - startIdx + 1))
            {
                f.IsSelectionModeSelected = true;
            }
        }
    }
}