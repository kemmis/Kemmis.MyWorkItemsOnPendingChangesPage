﻿using Microsoft.TeamFoundation.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    [TeamExplorerSection(SectionId, TeamExplorerPageIds.GitChanges, 25)]
    internal class GitChangesMyWorkItemsSection : MyWorkItemsSection
    {
        public const string SectionId = "4C82595C-9E77-467E-9F25-D886E694C999";

        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            return new GitChangesMyWOrkItemsSectionViewModel();
        }
    }
}