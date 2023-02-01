using System.Windows.Controls;
using System.Windows.Input;
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
            var lvDataContext = lv?.DataContext as IListViewControllable;
            var fileContainer = lvDataContext?.FileContainer;

            if (lvDataContext == null || fileContainer == null)
            {
                return;
            }

            if (e.Key == Key.J)
            {
                var amount = fileContainer.ExecuteCounter != 0 ? fileContainer.ExecuteCounter : 1;
                fileContainer.ExecuteCounter = 0;
                fileContainer.DownCursor(amount);
            }

            if (e.Key == Key.K)
            {
                var amount = fileContainer.ExecuteCounter != 0 ? fileContainer.ExecuteCounter : 1;
                fileContainer.ExecuteCounter = 0;
                fileContainer.UpCursor(amount);
            }

            if (e.Key == Key.D)
            {
                var amount = fileContainer.ExecuteCounter != 0 ? fileContainer.ExecuteCounter : 1;
                fileContainer.ExecuteCounter = 0;
                var itemCountPerPage = (int)(lv.ActualHeight / lvDataContext.ListViewItemLineHeight);
                fileContainer.DownCursor(amount * itemCountPerPage);
            }

            if (e.Key == Key.U)
            {
                var amount = fileContainer.ExecuteCounter != 0 ? fileContainer.ExecuteCounter : 1;
                fileContainer.ExecuteCounter = 0;
                var itemCountPerPage = (int)(lv.ActualHeight / lvDataContext.ListViewItemLineHeight);
                fileContainer.UpCursor(amount * itemCountPerPage);
            }

            if (e.Key == Key.G)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                {
                    fileContainer.JumpToLast();
                    var selectedItem = lv.SelectedItem;
                    lv.SelectedItem = null;
                    lv.SelectedItem = selectedItem;
                }
                else
                {
                    fileContainer.JumpToHead();
                }
            }

            if ((int)e.Key >= (int)Key.D0 && (int)e.Key <= (int)Key.D9)
            {
                var counterString = ((int)e.Key - (int)Key.D0).ToString();
                if (fileContainer.ExecuteCounter < 10000)
                {
                    fileContainer.ExecuteCounter = int.Parse(fileContainer.ExecuteCounter + counterString);
                }
            }
        }
    }
}