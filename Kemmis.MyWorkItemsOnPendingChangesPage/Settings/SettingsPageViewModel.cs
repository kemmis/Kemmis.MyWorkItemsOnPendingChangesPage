using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Settings
{
    [TeamExplorerPage(PageId)]
    public class SettingsPageViewModel : TeamExplorerBasePage
    {
        public const string PageId = "4C82595C-9E77-467E-9F25-D886E694C363";
        public SettingsPageViewModel()
        {
            Title = "My Work Items Settings";
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            PageContent = new SettingsPageView(ServiceProvider);
        }
    }
}
