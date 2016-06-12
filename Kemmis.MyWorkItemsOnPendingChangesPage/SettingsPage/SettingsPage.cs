using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.TeamExplorerBase;
using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.SettingsPage
{
    [TeamExplorerPage(PageId)]
    public class SettingsPage : TeamExplorerBasePage
    {
        public const string PageId = "4C82595C-9E77-467E-9F25-D886E694C363";
        public SettingsPage()
        {
            Title = "My Work Items Settings";
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            PageContent = new SettingsView(ServiceProvider);
        }
    }
}
