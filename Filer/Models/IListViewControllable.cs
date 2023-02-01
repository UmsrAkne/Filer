namespace Filer.Models
{
    public interface IListViewControllable
    {
        FileContainer FileContainer { get; }

        // PageDown ,PageUp の操作を行う際に ListViewItem 一行あたりの高さが必要になる。
        double ListViewItemLineHeight { get; }

        string CommandText { get; set; }
    }
}