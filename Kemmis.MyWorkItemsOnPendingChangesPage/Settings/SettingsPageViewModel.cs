using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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

        private RelayCommand _addStatusCommand;

        private RelayCommand _addTypeCommand;

        private RelayCommand _cancelCommand;

        private SortedObservableRangeCollection<SettingItemModel> _columns;
        private readonly object _columnsLock = new object();
        private int _daysBackToQuery;
        private int _maxWorkItems;

        private RelayCommand _refreshStatusesCommand;

        private RelayCommand _refreshTypesCommand;

        private RelayCommand _removeStatusCommand;

        private RelayCommand _removeTypeCommand;

        private RelayCommand _saveCommand;
        private SettingItemModel _selectedStatus;

        private SettingItemModel _selectedType;
        private SettingsRepository _settingsRepository;
        private readonly object _statusesLock = new object();
        private string _statusToAdd;
        private readonly object _typesLock = new object();
        private string _typeToAdd;
        private WorkItemRepository _workItemRepository;
        private SortedObservableRangeCollection<SettingItemModel> _workItemStatuses;
        private SortedObservableRangeCollection<SettingItemModel> _workItemTypes;

        public SettingsPageViewModel()
        {
            Title = "My Work Items Settings";
        }

        public int MaxWorkItems
        {
            get => _maxWorkItems;
            set
            {
                if (_maxWorkItems != value)
                {
                    _maxWorkItems = value;
                    RaisePropertyChanged("MaxWorkItems");
                }
            }
        }

        public int DaysBackToQuery
        {
            get => _daysBackToQuery;
            set
            {
                if (_daysBackToQuery != value)
                {
                    _daysBackToQuery = value;
                    RaisePropertyChanged("DaysBackToQuery");
                }
            }
        }

        public SortedObservableRangeCollection<SettingItemModel> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new SortedObservableRangeCollection<SettingItemModel>();
                    BindingOperations.EnableCollectionSynchronization(_columns, _columnsLock);
                }

                return _columns;
            }
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                    RaisePropertyChanged("Columns");
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
            get => _typeToAdd;
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
            get => _statusToAdd;
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
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    RaisePropertyChanged("SelectedStatus");
                }
            }
        }

        public SettingItemModel SelectedType
        {
            get => _selectedType;
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

        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Close));
        public RelayCommand AddStatusCommand => _addStatusCommand ?? (_addStatusCommand = new RelayCommand(AddStatus));

        public RelayCommand RemoveStatusCommand =>
            _removeStatusCommand ?? (_removeStatusCommand = new RelayCommand(RemoveStatus));

        public RelayCommand AddTypeCommand => _addTypeCommand ?? (_addTypeCommand = new RelayCommand(AddType));

        public RelayCommand RemoveTypeCommand =>
            _removeTypeCommand ?? (_removeTypeCommand = new RelayCommand(RemoveType));

        public RelayCommand RefreshTypesCommand =>
            _refreshTypesCommand ?? (_refreshTypesCommand = new AsyncRelayCommand(RefreshTypes));

        public RelayCommand RefreshStatusesCommand =>
            _refreshStatusesCommand ?? (_refreshStatusesCommand = new AsyncRelayCommand(RefreshStatuses));

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            _settingsRepository = new SettingsRepository(e.ServiceProvider);
            _workItemRepository = new WorkItemRepository(e.ServiceProvider);
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
            Columns.AddRange(settings.Columns);
            DaysBackToQuery = settings.DaysBackToQuery;
            MaxWorkItems = settings.MaxWorkItems;

            if (!WorkItemTypes.Any()) await LoadTypesFromServer();

            if (!WorkItemStatuses.Any()) await LoadStatusesFromServer();

            if (!Columns.Any()) await LoadColumnDefaults();
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

        private async Task LoadColumnDefaults()
        {
            IsBusy = true;
            await _workItemRepository.GetColumnsAsync(Columns);
            IsBusy = false;
        }

        private void AddType()
        {
            if (!string.IsNullOrWhiteSpace(TypeToAdd))
            {
                WorkItemTypes.Add(new SettingItemModel
                {
                    Checked = true,
                    Name = TypeToAdd
                });
                TypeToAdd = string.Empty;
            }
        }

        private void RemoveType()
        {
            if (SelectedType != null) WorkItemTypes.Remove(SelectedType);
        }

        private Task RefreshTypes()
        {
            return LoadTypesFromServer();
        }

        private void AddStatus()
        {
            if (!string.IsNullOrWhiteSpace(StatusToAdd))
            {
                WorkItemStatuses.Add(new SettingItemModel
                {
                    Checked = true,
                    Name = StatusToAdd
                });
                StatusToAdd = string.Empty;
            }
        }

        private void RemoveStatus()
        {
            if (SelectedStatus != null) WorkItemStatuses.Remove(SelectedStatus);
        }

        private Task RefreshStatuses()
        {
            return LoadStatusesFromServer();
        }

        private void Save()
        {
            var settings = new SettingsModel
            {
                DaysBackToQuery = DaysBackToQuery,
                MaxWorkItems = MaxWorkItems,
                WorkItemTypes = WorkItemTypes.ToList(),
                WorkItemStatuses = WorkItemStatuses.ToList(),
                Columns = Columns.ToList()
            };

            _settingsRepository.SaveSettingsAsync(settings);

            Close();
        }

        public void Close()
        {
            // Navigate to the settings page
            var teamExplorer = GetService<ITeamExplorer>();
            if (teamExplorer != null) teamExplorer.CurrentPage.Close();
        }
    }
}