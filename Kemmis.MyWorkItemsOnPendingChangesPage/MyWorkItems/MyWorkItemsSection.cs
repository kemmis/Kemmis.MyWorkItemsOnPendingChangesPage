using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    internal abstract class MyWorkItemsSection : TeamExplorerSectionBase
    {
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            return new MyWorkItemsSectionViewModel();
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            var view = new MyWorkItemsSectionView();
            return view;
        }

        protected override void InitializeView(SectionInitializeEventArgs e)
        {
            base.InitializeView(e);
            var view = base.View as MyWorkItemsSectionView;
            // do intit stuff here if needed
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            if (base.ViewModel != null && base.ViewModel is MyWorkItemsSectionViewModel && base.View != null && base.View is MyWorkItemsSectionView)
            {
                var viewModel = (MyWorkItemsSectionViewModel)base.ViewModel;
                var view = (MyWorkItemsSectionView)base.View;

                // add some commands if needed

                //workItemsSectionViewModel.SectionCommands = new ITeamExplorerSectionCommand[2]
                //{
                //    new TeamExplorerSectionCommand(workItemsSectionViewModel.ToggleAddByIdCommand, GitControlsResources.Get("CommandLinkText_AddWorkItemById"), WpfUtil.SharedResources["PlusIconBrush"] as DrawingBrush),
                //    new TeamExplorerSectionCommand(workItemsSectionView.ShowQueriesMenuCommand, GitControlsResources.Get("CommandLinkText_Queries"), WpfUtil.SharedResources["FlatListIconBrush"] as DrawingBrush)
                //};
            }
        }
    }
}
