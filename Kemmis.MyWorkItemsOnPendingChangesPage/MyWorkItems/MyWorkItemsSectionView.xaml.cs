using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    /// <summary>
    ///     Interaction logic for MyWorkItemsSectionView.xaml
    /// </summary>
    public partial class MyWorkItemsSectionView : UserControl
    {
        private MyWorkItemsSectionViewModel _viewModel;

        public MyWorkItemsSectionView()
        {
            InitializeComponent();
            DataContextChanged += MyWorkItemsSectionView_DataContextChanged;
            Loaded += MyWorkItemsSectionView_Loaded;
        }

        private void MyWorkItemsSectionView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.OnViewLoadedCommand.Execute(null);
        }

        private void MyWorkItemsSectionView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as MyWorkItemsSectionViewModel;
        }


        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control) sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}