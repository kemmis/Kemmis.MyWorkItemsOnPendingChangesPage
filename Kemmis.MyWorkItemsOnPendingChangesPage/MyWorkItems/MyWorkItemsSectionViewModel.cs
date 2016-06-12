using System;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Kemmis.MyWorkItemsOnPendingChangesPage.Settings;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    [TeamExplorerSection(SectionId, TeamExplorerPageIds.PendingChanges, 900)]
    public class MyWorkItemsSectionViewModel : TeamExplorerBaseSection
    {
        private RelayCommand _navSettingsCommand;
        public const string SectionId = "4C82595C-9E77-467E-9F25-D886E694C361";

        public MyWorkItemsSectionViewModel()
        {
            Title = "My Work Items";
            IsExpanded = true;
            IsBusy = false;
            var view = new MyWorkItemsSectionView();
            SectionContent = view;
            view.DataContext = this;
        }

        public RelayCommand NavSettingsCommand => _navSettingsCommand ?? (_navSettingsCommand = new RelayCommand(NavigateToSettingsPage));

        public void NavigateToSettingsPage()
        {
            // Navigate to the settings page
            var teamExplorer = GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(SettingsPageViewModel.PageId), null);
            }
        }
    }
}
