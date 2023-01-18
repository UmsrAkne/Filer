using System.Windows;
using System.Windows.Input;
using Filer.ViewModels;
using Filer.Views;
using Microsoft.Xaml.Behaviors;

namespace Filer.Models.Behaviors
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

            //// ここからビューの操作に関するメソッド群

            if (e.Key == Key.H)
            {
                vm.FocusToListView(window.LeftListView.ListView);
            }

            if (e.Key == Key.L)
            {
                vm.FocusToListView(window.RightListView.ListView);
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

            if (e.Key == Key.Return && (Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                vm.OpenFolderToAnotherListView();
            }

            //// ここからファイル操作に関するメソッドの呼び出し

            if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                vm.DeleteFile();
            }

            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                vm.CopyFile();
            }

            if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                vm.MoveFile();
            }
        }
    }
}