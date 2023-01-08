using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Filer.ViewModels;

namespace Filer.Views
{
    /// <summary>
    /// UserListView.xaml の相互作用ロジック
    /// </summary>
    public partial class UserListView
    {
        public static readonly DependencyProperty PathBarTextProperty =
            DependencyProperty.Register(nameof(PathBarText), typeof(string), typeof(UserListView), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ListSourceProperty =
            DependencyProperty.Register(nameof(ListSource), typeof(IEnumerable), typeof(UserListView), new PropertyMetadata(null));

        public UserListView()
        {
            DataContextChanged += (sender, args) =>
            {
                if (DataContext is FileListViewModel model)
                {
                    model.ListView = ListView;
                }
            };

            InitializeComponent();
        }

        public string PathBarText
        {
            get => (string)GetValue(PathBarTextProperty);
            set => SetValue(PathBarTextProperty, value);
        }

        public IEnumerable ListSource
        {
            get => (IEnumerable)GetValue(ListSourceProperty);
            set => SetValue(ListSourceProperty, value);
        }

        public ListView ListView => FindName("listView") as ListView;
    }
}