using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Services
{
    public class WorkItemRepository
    {
        private ITeamFoundationContext _context;
        public WorkItemRepository(ITeamFoundationContext context)
        {
            _context = context;
        }

        public async Task<List<SettingItemModel>> GetWorkItemTypesAsync()
        {
            return await Task.Run(() =>
            {
                if (_context != null && _context.HasCollection && _context.HasTeamProject)
                {
                    var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();

                    var types = new List<string>();

                    foreach (Project p in wis.Projects)
                    {
                        foreach (WorkItemType t in p.WorkItemTypes)
                        {

                            types.Add(t.Name);
                        }
                    }

                    return types.OrderBy(t => t).Distinct().Select(t => new SettingItemModel()
                    {
                        Name = t,
                        Checked = true
                    }).ToList();
                }
                return null;
            });
        }

        public async Task GetWorkItemStatesAsync(ObservableCollection<SettingItemModel> collection)
        {
            await Task.Run(() =>
            {
                if (_context != null && _context.HasCollection && _context.HasTeamProject)
                {
                    var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();
                    var workItems = wis.Query("Select [State], [Title] " +
                                              "From WorkItems " +
                                              "Order By [State] Asc, [Changed Date] Desc");


                    foreach (WorkItem w in workItems)
                    {
                        if (collection.All(s => s.Name != w.State))
                        {
                            collection.Add(new SettingItemModel { Name = w.State });
                        }

                        foreach (Revision rev in w.Revisions)
                        {
                            var state = rev.Fields["State"].Value as string;

                            if (collection.All(s => s.Name != state))
                            {
                                collection.Add(new SettingItemModel { Name = state });
                            }
                        }
                    }
                };
            });
        }
    }
}
