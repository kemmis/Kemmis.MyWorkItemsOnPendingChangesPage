using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private object _typesLock = new object();

        public int DaysBackToQuery
        {
            get
            {
                return _daysBackToQuery;
            }
            set
            {
                if (_daysBackToQuery != value)
                {
                    _daysBackToQuery = value;
                    RaisePropertyChanged("DaysBackToQuery");
                }
            }
        }

        public SortedObservableRangeCollection<SettingItemModel> WorkItemTypes
        {
            get
            {
                if (_workItemTypes == null)
                {
                    _workItemTypes = new SortedObservableRangeCollection<SettingItemModel>();
                    BindingOperations.EnableCollectionSynchronization(_workItemTypes, _typesLock);
                }
                return _workItemTypes;
            }
            set
            {
                if (_workItemTypes != value)
                {
                    _workItemTypes = value;
                    RaisePropertyChanged("WorkItemTypes");
                }
            }
        }

        public string TypeToAdd
        {
            get { return _typeToAdd; }
            set
            {
                if (_typeToAdd != value)
                {
                    _typeToAdd = value;
                    RaisePropertyChanged("TypeToAdd");
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

        private SettingItemModel _selectedType;
        public SettingItemModel SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    RaisePropertyChanged("SelectedType");
                }
            }
        }

        public SortedObservableRangeCollection<SettingItemModel> WorkItemStatuses
        {
            get
            {
                if (_workItemStatuses == null)
                {
                    _workItemStatuses = new SortedObservableRangeCollection<SettingItemModel>();
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
            await LoadSavedState();
        }

        private async Task LoadSavedState()
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            WorkItemTypes.AddRange(settings.WorkItemTypes);
            WorkItemStatuses.AddRange(settings.WorkItemStatuses);
            DaysBackToQuery = settings.DaysBackToQuery;

            
            if (!WorkItemTypes.Any())
            {
                await LoadTypesFromServer();
            }

            if (!WorkItemStatuses.Any())
            {
                await LoadStatusesFromServer();
            }
        }

        private async Task LoadTypesFromServer()
        {
            IsBusy = true;
            await _workItemRepository.GetWorkItemTypesAsync(WorkItemTypes);
            IsBusy = false;
        }

        private async Task LoadStatusesFromServer()
        {
            IsBusy = true;
            await _workItemRepository.GetWorkItemStatesAsync(WorkItemStatuses);
            IsBusy = false;
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private RelayCommand _cancelCommand;
        private SortedObservableRangeCollection<SettingItemModel> _workItemTypes;
        private SortedObservableRangeCollection<SettingItemModel> _workItemStatuses;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Close));

        private RelayCommand _addStatusCommand;
        public RelayCommand AddStatusCommand => _addStatusCommand ?? (_addStatusCommand = new RelayCommand(AddStatus));

        private RelayCommand _removeStatusCommand;
        public RelayCommand RemoveStatusCommand => _removeStatusCommand ?? (_removeStatusCommand = new RelayCommand(RemoveStatus));

        private RelayCommand _addTypeCommand;
        public RelayCommand AddTypeCommand => _addTypeCommand ?? (_addTypeCommand = new RelayCommand(AddType));

        private RelayCommand _removeTypeCommand;
        public RelayCommand RemoveTypeCommand => _removeTypeCommand ?? (_removeTypeCommand = new RelayCommand(RemoveType));

        private RelayCommand _refreshTypesCommand;
        public RelayCommand RefreshTypesCommand => _refreshTypesCommand ?? (_refreshTypesCommand = new AsyncRelayCommand(RefreshTypes));

        private RelayCommand _refreshStatusesCommand;
        private string _statusToAdd;
        private SettingItemModel _selectedStatus;
        private int _daysBackToQuery;
        private string _typeToAdd;
        public RelayCommand RefreshStatusesCommand => _refreshStatusesCommand ?? (_refreshStatusesCommand = new AsyncRelayCommand(RefreshStatuses));

        private void AddType()
        {
            if (!string.IsNullOrWhiteSpace(TypeToAdd))
            {
                WorkItemTypes.Add(new SettingItemModel()
                {
                    Checked = true,
                    Name = TypeToAdd
                });
                TypeToAdd = string.Empty;
            }
        }

        private void RemoveType()
        {
            if (SelectedType != null)
            {
                WorkItemTypes.Remove(SelectedType);
            }
        }

        private Task RefreshTypes()
        {
            return LoadTypesFromServer();
        }

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

        private Task RefreshStatuses()
        {
            return LoadStatusesFromServer();
        }

        private void Save()
        {
            var settings = new SettingsModel()
            {
                DaysBackToQuery = DaysBackToQuery,
                WorkItemTypes = WorkItemTypes.ToList(),
                WorkItemStatuses = WorkItemStatuses.ToList()
            };

            _settingsRepository.SaveSettingsAsync(settings);

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
