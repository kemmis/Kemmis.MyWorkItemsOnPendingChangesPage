using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Kemmis.MyWorkItemsOnPendingChangesPage.Common;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Kemmis.MyWorkItemsOnPendingChangesPage.Services;
using Kemmis.MyWorkItemsOnPendingChangesPage.Settings;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.MVVM;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    internal class MyWorkItemsSectionViewModel : TeamExplorerSectionViewModelBase
    {
        private RelayCommand<WorkItemModel> _addWorkItemCommand;
        private bool _isConfigured;
        private RelayCommand _navSettingsCommand;
        private AsyncRelayCommand _onLoadedCommand;
        private RelayCommand<WorkItemModel> _openWorkItemCommand;
        private AsyncRelayCommand _refreshCommand;

        private SettingsModel _settings;


        private SettingsRepository _settingsRepository;
        private WorkItemRepository _workItemRepository;
        private ObservableRangeCollection<WorkItemModel> _workItems;
        private readonly object _workItemsLock = new object();

        public MyWorkItemsSectionViewModel()
        {
            Title = "My Work Items";
            IsExpanded = true;
            IsBusy = false;
        }

        public bool IsConfigured
        {
            get => _isConfigured;
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

        public AsyncRelayCommand OnViewLoadedCommand
            => _onLoadedCommand ?? (_onLoadedCommand = new AsyncRelayCommand(ViewOnLoaded));

        public RelayCommand NavSettingsCommand
            => _navSettingsCommand ?? (_navSettingsCommand = new RelayCommand(NavigateToSettingsPage));

        public AsyncRelayCommand RefreshCommand
            => _refreshCommand ?? (_refreshCommand = new AsyncRelayCommand(LoadWorkItems));

        public RelayCommand<WorkItemModel> AddWorkItemCommand
            => _addWorkItemCommand ?? (_addWorkItemCommand = new RelayCommand<WorkItemModel>(AddWorkItem));

        public RelayCommand<WorkItemModel> OpenWorkItemCommand
            => _openWorkItemCommand ?? (_openWorkItemCommand = new RelayCommand<WorkItemModel>(OpenWorkItem));

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);


            // need to get ITeamFoundationContextManager
            //var view = new MyWorkItemsSectionView();
            //SectionContent = view; // how to init with mvvm?
            //view.DataContext = this;
            //view.Loaded += ViewOnLoaded;
        }

        public async Task ViewOnLoaded()
        {
            _settingsRepository = new SettingsRepository(ServiceProvider);
            _workItemRepository = new WorkItemRepository(ServiceProvider);
            await LoadWorkItems();
        }


        public virtual void AddWorkItem(WorkItemModel workItemModel)
        {
            try
            {
                if (workItemModel == null)
                    return;

                var selectedWorkItemId = workItemModel.Id;

                var pc = ResolveService<IPendingChangesExt>();
                var model = pc.GetType().GetField("m_workItemsSection", BindingFlags.NonPublic | BindingFlags.Instance);
                var t = model.FieldType;
                var mm = model.GetValue(pc);
                var m = t.GetMethod("AddWorkItemById", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(mm, new object[] {selectedWorkItemId});
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

            var selectedWorkItemId = workItemModel.Id;

            IWorkItemDocument widoc = null;
            try
            {
                var documentService = ResolveService<DocumentService>();
                var manager = ResolveService<ITeamFoundationContextManager>();

                widoc = documentService.GetWorkItem(manager.CurrentContext.TeamProjectCollection, selectedWorkItemId,
                    this);
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
            var teamExplorer = ResolveService<ITeamExplorer>();
            if (teamExplorer != null) teamExplorer.NavigateToPage(new Guid(SettingsPageViewModel.PageId), null);
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

            if (!_settings.Columns.Any()) await _workItemRepository.GetColumnsAsync(_settings.Columns);

            RaisePropertyChanged("ShowIdColumn");
            RaisePropertyChanged("ShowWITColumn");
            RaisePropertyChanged("ShowTitleColumn");
            RaisePropertyChanged("ShowStateColumn");
            RaisePropertyChanged("ShowAssignedToColumn");

            IsBusy = false;
        }

        public TService ResolveService<TService>() where TService : class
        {
            return GetService<TService>() ?? base.ResolveService<TService>();
        }

        private T GetService<T>()
        {
            if (ServiceProvider != null) return (T) ServiceProvider.GetService(typeof(T));
            return default(T);
        }
    }

    public class GridViewColumnExt : GridViewColumn
    {
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility),
                typeof(GridViewColumnExt),
                new FrameworkPropertyMetadata(Visibility.Visible,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnVisibilityPropertyChanged));

        private double _visibleWidth;

        public Visibility Visibility
        {
            get => (Visibility) GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }

        private static void OnVisibilityPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var column = d as GridViewColumnExt;
            if (column != null) column.OnVisibilityChanged((Visibility) e.NewValue);
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
    }
}