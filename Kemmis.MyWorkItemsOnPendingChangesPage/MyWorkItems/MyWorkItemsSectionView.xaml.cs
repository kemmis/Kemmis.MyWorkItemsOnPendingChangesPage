using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    /// <summary>
    /// Interaction logic for MyWorkItemsSectionView.xaml
    /// </summary>
    public partial class MyWorkItemsSectionView : UserControl
    {
        public MyWorkItemsSectionView()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
