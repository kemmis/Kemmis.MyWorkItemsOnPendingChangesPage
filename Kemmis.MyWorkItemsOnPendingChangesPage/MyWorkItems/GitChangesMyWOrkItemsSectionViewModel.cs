using System;
using System.Reflection;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Microsoft.TeamFoundation.Controls;

//using Microsoft.TeamFoundation.WorkItemTracking.WpfControls;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.MyWorkItems
{
    internal class GitChangesMyWOrkItemsSectionViewModel : MyWorkItemsSectionViewModel
    {
        public override void AddWorkItem(WorkItemModel workItemModel)
        {
            try
            {
                if (workItemModel == null)
                    return;
                var GitPendingChangesModelType =
                    Type.GetType(
                        "Microsoft.TeamFoundation.Git.Controls.PendingChanges.GitPendingChangesModel,Microsoft.TeamFoundation.Git.Controls");
                var gitPendingChangesModel = ServiceProvider.GetService(GitPendingChangesModelType);
                var method = GitPendingChangesModelType.GetMethod("AddWorkItemsByIdAsync",
                    BindingFlags.Public | BindingFlags.Instance);
                method.Invoke(gitPendingChangesModel, new object[] {new[] {workItemModel.Id}});
            }
            catch (Exception ex)
            {
                ShowNotification(ex.ToString(), NotificationType.Error);
            }
        }
    }
}