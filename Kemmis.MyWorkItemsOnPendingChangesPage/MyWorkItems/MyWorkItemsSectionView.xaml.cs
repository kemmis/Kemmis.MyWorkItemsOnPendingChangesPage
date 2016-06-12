using System.Windows;
using System.Windows.Controls;

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

        public MyWorkItemsSectionViewModel ParentSectionViewModel
        {
            get { return (MyWorkItemsSectionViewModel)GetValue(ParentSectionViewModelProperty); }
            set { SetValue(ParentSectionViewModelProperty, value); }
        }
        public static readonly DependencyProperty ParentSectionViewModelProperty =
            DependencyProperty.Register("ParentSectionViewModel", typeof(MyWorkItemsSectionViewModel), typeof(MyWorkItemsSectionView));

        private void SettingsLink_OnClick(object sender, RoutedEventArgs e)
        {
            ParentSectionViewModel.NavigateToSettingsPage();
        }
    }
}
