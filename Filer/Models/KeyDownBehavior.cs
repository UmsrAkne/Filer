using System.Windows;
using System.Windows.Input;
using Filer.ViewModels;
using Filer.Views;
using Microsoft.Xaml.Behaviors;

namespace Filer.Models
{
    public class KeyDownBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObjectOnKeyDown;
        }

        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            var window = sender as MainWindow;
            var vm = window?.DataContext as MainWindowViewModel;

            if (vm == null || vm.TextInputting)
            {
                return;
            }

            if (e.Key == Key.H)
            {
                vm.FocusToListView(window.leftListView.ListView);
            }

            if (e.Key == Key.L)
            {
                vm.FocusToListView(window.rightListView.ListView);
            }

            if (e.Key == Key.S)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                {
                    vm.SyncFromAnother();
                }
                else
                {
                    vm.SyncToAnother();
                }
            }

            if (e.Key == Key.X)
            {
                vm.SwitchFileListView();
            }
        }
    }
}