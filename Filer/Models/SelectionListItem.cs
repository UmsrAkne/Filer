using System.Windows.Input;

namespace Filer.Models
{
    public class SelectionListItem
    {
        public Key Key { get; set; }

        public string KeyText => Key.ToString();

        public string Header { get; set; }

        public int Index { get; set; }
    }
}