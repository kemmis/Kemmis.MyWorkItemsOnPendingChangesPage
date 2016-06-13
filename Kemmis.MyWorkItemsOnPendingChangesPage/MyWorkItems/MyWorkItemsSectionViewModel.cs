using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Kemmis.MyWorkItemsOnPendingChangesPage.Services;
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
        private SettingsRepository _settingsRepository;
        private WorkItemRepository _workItemRepository;
        private ObservableRangeCollection<WorkItemModel> _workItems;
        private object _workItemsLock = new object();
        public ObservableRangeCollection<WorkItemModel> WorkItems
        {
            get
            {
                if (_workItems == null)
                {
                    _workItems = new ObservableRangeCollection<WorkItemModel>();
                    BindingOperations.EnableCollectionSynchronization(_workItems, _workItemsLock);
                }
                return _workItems;
            }
            set
            {
                if (_workItems != value)
                {
                    _workItems = value;
                    RaisePropertyChanged("WorkItems");
                }
            }
        }

        public MyWorkItemsSectionViewModel()
        {
            Title = "My Work Items";
            IsExpanded = true;
            IsBusy = false;

        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            _settingsRepository = new SettingsRepository(e.ServiceProvider);
            _workItemRepository = new WorkItemRepository(CurrentContext);
            var view = new MyWorkItemsSectionView();
            SectionContent = view;
            view.DataContext = this;
            view.Loaded += ViewOnLoaded;
        }

        private async void ViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await LoadWorkItems();
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

        public async Task LoadWorkItems()
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            await _workItemRepository.GetWorkItemsAsync(WorkItems, settings);
        }
    }
}
