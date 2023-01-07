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
                vm.FocusToListViewCommand.Execute(window.leftListView.ListView);
            }

            if (e.Key == Key.L)
            {
                vm.FocusToListViewCommand.Execute(window.rightListView.ListView);
            }

            if (e.Key == Key.S)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                {
                    vm.SyncFromAnotherCommand.Execute();
                }
                else
                {
                    vm.SyncToAnotherCommand.Execute();
                }
            }

            if (e.Key == Key.X)
            {
                vm.SwitchFileListVmCommand.Execute();
            }
        }
    }
}