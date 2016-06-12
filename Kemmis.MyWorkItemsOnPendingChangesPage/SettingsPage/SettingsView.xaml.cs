using System;
using System.Windows.Controls;
using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.SettingsPage
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly IServiceProvider _serviceProvider;

        public SettingsView(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Navigate to the settings page
            var teamExplorer = GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.CurrentPage.Close();
            }
        }

        public T GetService<T>()
        {
            if (this._serviceProvider != null)
            {
                return (T)this._serviceProvider.GetService(typeof(T));
            }
            return default(T);
        }
    }
}
