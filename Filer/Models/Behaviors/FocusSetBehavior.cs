using System.Windows;
using System.Windows.Controls;
using Filer.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace Filer.Models.Behaviors
{
    public class FocusSetBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LostFocus += AssociatedObjectOnLostFocus;
            AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.LostFocus -= AssociatedObjectOnLostFocus;
            AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus;
        }

        private void AssociatedObjectOnGotFocus(object sender, RoutedEventArgs e)
        {
            if (((UserControl)sender).DataContext is FileListViewModel vm)
            {
                vm.IsFocused = true;
            }
        }

        private void AssociatedObjectOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (((UserControl)sender).DataContext is FileListViewModel vm)
            {
                vm.IsFocused = false;
            }
        }
    }
}