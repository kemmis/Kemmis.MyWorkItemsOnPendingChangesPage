using System;
using System.Collections.Generic;
using System.Windows;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Kemmis.MyWorkItemsOnPendingChangesPage.Services;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Settings
{
    [TeamExplorerPage(PageId)]
    public class SettingsPageViewModel : TeamExplorerBasePage
    {
        public const string PageId = "4C82595C-9E77-467E-9F25-D886E694C363";
        private SettingsRepository _settingsRepository;
        private WorkItemRepository _workItemRepository;

        public List<SettingItemModel> WorkItemTypes
        {
            get { return _workItemTypes; }
            set
            {
                if (_workItemTypes != value)
                {
                    _workItemTypes = value;
                    RaisePropertyChanged("WorkItemTypes");
                }
            }
        }

        public SettingsPageViewModel()
        {
            Title = "My Work Items Settings";
            
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            _settingsRepository = new SettingsRepository(e.ServiceProvider);
            _workItemRepository = new WorkItemRepository(CurrentContext);
            var view = new SettingsPageView();
            PageContent = view;
            view.DataContext = this;
            view.Loaded += ViewOnLoaded;
        }

        private async void ViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WorkItemTypes = await _workItemRepository.GetWorkItemTypesAsync();
            var statuses = await _workItemRepository.GetWorkItemStatesAsync();
            var settings = await _settingsRepository.GetSettingsAsync();
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private RelayCommand _cancelCommand;
        private List<SettingItemModel> _workItemTypes;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Close));

        private void Save()
        {
            Close();
        }

        public void Close()
        {
            // Navigate to the settings page
            var teamExplorer = GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.CurrentPage.Close();
            }
        }
    }
}
