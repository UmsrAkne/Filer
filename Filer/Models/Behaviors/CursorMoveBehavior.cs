using System.Windows.Controls;
using System.Windows.Input;
using Filer.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace Filer.Models.Behaviors
{
    public class CursorMoveBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObjectKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObjectKeyDown;
        }

        private void AssociatedObjectKeyDown(object sender, KeyEventArgs e)
        {
            var lv = sender as ListView;
            var vm = lv?.DataContext as HistoryPageViewModel;
        }
    }
}