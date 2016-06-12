using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.TeamExplorerBase;
using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItemsSection
{
    [TeamExplorerSection(SectionId, TeamExplorerPageIds.PendingChanges, 900)]
    public class MyWorkItemsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "4C82595C-9E77-467E-9F25-D886E694C361";

        public MyWorkItemsSection()
        {
            Title = "My Work Items";
            IsExpanded = true;
            IsBusy = false;
            SectionContent = new MyWorkItemsView();
            View.ParentSection = this;
        }

        protected MyWorkItemsView View
        {
            get { return this.SectionContent as MyWorkItemsView; }
        }

        public void NavigateToSettingsPage()
        {
            // Navigate to the settings page
            var teamExplorer = GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(SettingsPage.SettingsPage.PageId), null);
            }
        }
    }
}
