namespace Filer.Views
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// UserListView.xaml の相互作用ロジック
    /// </summary>
    public partial class UserListView : UserControl
    {
        public static readonly DependencyProperty PathBarTextProperty =
            DependencyProperty.Register("PathBarText", typeof(string), typeof(UserListView), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ListSourceProperty =
            DependencyProperty.Register("ListSource", typeof(IEnumerable), typeof(UserListView), new PropertyMetadata(null));

        public UserListView()
        {
            InitializeComponent();
        }

        public string PathBarText
        {
            get { return (string)GetValue(PathBarTextProperty); }
            set { SetValue(PathBarTextProperty, value); }
        }

        public IEnumerable ListSource
        {
            get { return (IEnumerable)GetValue(ListSourceProperty); }
            set { SetValue(ListSourceProperty, value); }
        }

        public ListView ListView
        {
            get => FindName("listView") as ListView;
        }
    }
}
