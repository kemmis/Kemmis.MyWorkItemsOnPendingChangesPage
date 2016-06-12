using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItemsSection
{
    /// <summary>
    /// Interaction logic for MyWorkItemsView.xaml
    /// </summary>
    public partial class MyWorkItemsView : UserControl
    {
        public MyWorkItemsView()
        {
            InitializeComponent();
        }

        public MyWorkItemsSection ParentSection
        {
            get { return (MyWorkItemsSection)GetValue(ParentSectionProperty); }
            set { SetValue(ParentSectionProperty, value); }
        }
        public static readonly DependencyProperty ParentSectionProperty =
            DependencyProperty.Register("ParentSection", typeof(MyWorkItemsSection), typeof(MyWorkItemsView));

        private void SettingsLink_OnClick(object sender, RoutedEventArgs e)
        {
            ParentSection.NavigateToSettingsPage();
        }
    }
}
