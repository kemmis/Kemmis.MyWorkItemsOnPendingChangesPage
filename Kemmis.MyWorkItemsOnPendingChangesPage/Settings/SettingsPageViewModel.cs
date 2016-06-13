using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Kemmis.MyWorkItemsOnPendingChangesPage.Services;
using Microsoft.Internal.VisualStudio.PlatformUI;
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
        private object _statusesLock = new object();

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

        public string StatusToAdd
        {
            get { return _statusToAdd; }
            set
            {
                if (_statusToAdd != value)
                {
                    _statusToAdd = value;
                    RaisePropertyChanged("StatusToAdd");
                }
            }
        }

        public SettingItemModel SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    RaisePropertyChanged("SelectedStatus");
                }
            }
        }

        public ObservableCollection<SettingItemModel> WorkItemStatuses
        {
            get
            {
                if (_workItemStatuses == null)
                {
                    _workItemStatuses = new ObservableCollection<SettingItemModel>();
                    BindingOperations.EnableCollectionSynchronization(_workItemStatuses, _statusesLock);
                }
                return _workItemStatuses;
            }
            set
            {
                if (_workItemStatuses != value)
                {
                    _workItemStatuses = value;
                    RaisePropertyChanged("WorkItemStatuses");
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
            await LoadStatusesFromServer();
            var settings = await _settingsRepository.GetSettingsAsync();
        }

        private async Task LoadStatusesFromServer()
        {
            await _workItemRepository.GetWorkItemStatesAsync(WorkItemStatuses);
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private RelayCommand _cancelCommand;
        private List<SettingItemModel> _workItemTypes;
        private ObservableCollection<SettingItemModel> _workItemStatuses;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Close));

        private RelayCommand _addStatusCommand;
        public RelayCommand AddStatusCommand => _addStatusCommand ?? (_addStatusCommand = new RelayCommand(AddStatus));

        private RelayCommand _removeStatusCommand;
        public RelayCommand RemoveStatusCommand => _removeStatusCommand ?? (_removeStatusCommand = new RelayCommand(RemoveStatus));

        private RelayCommand _refreshStatusesCommand;
        private string _statusToAdd;
        private SettingItemModel _selectedStatus;
        public RelayCommand RefreshStatusesCommand => _refreshStatusesCommand ?? (_refreshStatusesCommand = new AsyncRelayCommand(RefreshStatuses));

        private void AddStatus()
        {
            if (!string.IsNullOrWhiteSpace(StatusToAdd))
            {
                WorkItemStatuses.Add(new SettingItemModel()
                {
                    Checked = true,
                    Name = StatusToAdd
                });
                StatusToAdd = string.Empty;
            }
        }

        private void RemoveStatus()
        {
            if (SelectedStatus != null)
            {
                WorkItemStatuses.Remove(SelectedStatus);
            }
        }

        private async Task RefreshStatuses()
        {
            await LoadStatusesFromServer();
        }

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
