using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Filer.Models
{
    public class ScrollBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            lv?.ScrollIntoView(lv.SelectedItem);
        }
    }
}