using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common.ViewModelBaseClasses;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Kemmis.MyWorkItemsOnPendingChangesPage.Services;
using Kemmis.MyWorkItemsOnPendingChangesPage.Settings;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using System.Windows.Controls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    [TeamExplorerSection(SectionId, TeamExplorerPageIds.PendingChanges, 35)]
    public class MyWorkItemsSectionViewModel : TeamExplorerBaseSection
    {
        private RelayCommand _navSettingsCommand;
        private AsyncRelayCommand _refreshCommand;
        private RelayCommand<WorkItemModel> _addWorkItemCommand;
        private RelayCommand<WorkItemModel> _openWorkItemCommand;

        public const string SectionId = "4C82595C-9E77-467E-9F25-D886E694C361";
        private SettingsRepository _settingsRepository;
        private WorkItemRepository _workItemRepository;
        private ObservableRangeCollection<WorkItemModel> _workItems;
        private object _workItemsLock = new object();
        private bool _isConfigured;

        public bool IsConfigured
        {
            get { return _isConfigured; }
            set
            {
                if (_isConfigured != value)
                {
                    _isConfigured = value;
                    RaisePropertyChanged("IsConfigured");
                    RaisePropertyChanged("NeedsConfigured");
                }
            }
        }

        public bool NeedsConfigured => !IsConfigured;

        private SettingsModel _settings;

        private SettingsModel Settings => _settings ?? new SettingsModel();

        public bool ShowIdColumn => Settings.Columns.Any(c => c.Checked && c.Name == "Id");
        public bool ShowWITColumn => Settings.Columns.Any(c => c.Checked && c.Name == "Work Item Type");
        public bool ShowTitleColumn => Settings.Columns.Any(c => c.Checked && c.Name == "Title");
        public bool ShowStateColumn => Settings.Columns.Any(c => c.Checked && c.Name == "State");
        public bool ShowAssignedToColumn => Settings.Columns.Any(c => c.Checked && c.Name == "Assigned To");

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

        public RelayCommand NavSettingsCommand
            => _navSettingsCommand ?? (_navSettingsCommand = new RelayCommand(NavigateToSettingsPage));

        public AsyncRelayCommand RefreshCommand
            => _refreshCommand ?? (_refreshCommand = new AsyncRelayCommand(LoadWorkItems));

        public RelayCommand<WorkItemModel> AddWorkItemCommand
            => _addWorkItemCommand ?? (_addWorkItemCommand = new RelayCommand<WorkItemModel>(AddWorkItem));

        public RelayCommand<WorkItemModel> OpenWorkItemCommand
            => _openWorkItemCommand ?? (_openWorkItemCommand = new RelayCommand<WorkItemModel>(OpenWorkItem));



        public void AddWorkItem(WorkItemModel workItemModel)
        {
            try
            {
                if (workItemModel == null)
                    return;

                int selectedWorkItemId = workItemModel.Id;

                var pc = GetService<IPendingChangesExt>();
                var model = pc.GetType().GetField("m_workItemsSection", BindingFlags.NonPublic | BindingFlags.Instance);
                var t = model.FieldType;
                var mm = model.GetValue(pc);
                var m = t.GetMethod("AddWorkItemById", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(mm, new object[] { selectedWorkItemId });
            }
            catch (Exception ex)
            {
                ShowNotification(ex.ToString(), NotificationType.Error);
            }
        }

        public void OpenWorkItem(WorkItemModel workItemModel)
        {
            if (workItemModel == null)
                return;

            int selectedWorkItemId = workItemModel.Id;

            IWorkItemDocument widoc = null;
            try
            {
                var documentService = GetService<DocumentService>();
                widoc = documentService.GetWorkItem(CurrentContext.TeamProjectCollection, selectedWorkItemId, this);
                documentService.ShowWorkItem(widoc);
            }
            finally
            {
                if (widoc != null)
                    widoc.Release(this);
            }
        }

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
            IsBusy = true;

            _settings = await _settingsRepository.GetSettingsAsync();

            if (_settings.WorkItemStatuses.Any(s => s.Checked) && _settings.WorkItemTypes.Any(s => s.Checked))
            {
                IsConfigured = true;
                await _workItemRepository.GetWorkItemsAsync(WorkItems, _settings);
            }

            if (!_settings.Columns.Any())
            {
                await _workItemRepository.GetColumnsAsync(_settings.Columns);
            }

            RaisePropertyChanged("ShowIdColumn");
            RaisePropertyChanged("ShowWITColumn");
            RaisePropertyChanged("ShowTitleColumn");
            RaisePropertyChanged("ShowStateColumn");
            RaisePropertyChanged("ShowAssignedToColumn");

            IsBusy = false;
        }
    }

    public class GridViewColumnExt : GridViewColumn
    {
        public Visibility Visibility
        {
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
            set
            {
                SetValue(VisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility),
                typeof(GridViewColumnExt),
                new FrameworkPropertyMetadata(Visibility.Visible,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnVisibilityPropertyChanged)));

        private static void OnVisibilityPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var column = d as GridViewColumnExt;
            if (column != null)
            {
                column.OnVisibilityChanged((Visibility)e.NewValue);
            }
        }

        private void OnVisibilityChanged(Visibility visibility)
        {
            if (visibility == Visibility.Visible)
            {
                Width = _visibleWidth;
            }
            else
            {
                _visibleWidth = Width;
                Width = 0.0;
            }
        }

        double _visibleWidth;
    }
}
