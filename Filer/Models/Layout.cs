using System.Windows;
using Prism.Mvvm;

namespace Filer.Models
{
    public class Layout : BindableBase
    {
        private GridLength secondRowHeight;
        private int rightListViewRow;
        private int rightListViewColumn;
        private int columnSpan;

        public Layout()
        {
            ToHorizontalLayout();
        }

        public GridLength SecondRowHeight
        {
            get => secondRowHeight;
            private set => SetProperty(ref secondRowHeight, value);
        }

        public int RightListViewRow
        {
            get => rightListViewRow;
            private set => SetProperty(ref rightListViewRow, value);
        }

        public int RightListViewColumn
        {
            get => rightListViewColumn;
            private set => SetProperty(ref rightListViewColumn, value);
        }

        public int ColumnSpan
        {
            get => columnSpan;
            set => SetProperty(ref columnSpan, value);
        }

        public void ToHorizontalLayout()
        {
            SecondRowHeight = GridLength.Auto;
            RightListViewRow = 0;
            RightListViewColumn = 1;
            ColumnSpan = 1;
        }

        public void ToVerticalLayout()
        {
            SecondRowHeight = new GridLength(1.0, GridUnitType.Star);
            RightListViewRow = 1;
            RightListViewColumn = 0;
            ColumnSpan = 2;
        }
    }
}