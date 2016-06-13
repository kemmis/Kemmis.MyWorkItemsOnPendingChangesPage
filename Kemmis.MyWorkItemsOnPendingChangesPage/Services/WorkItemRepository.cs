using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Microsoft.TeamFoundation.Client;
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

                    return types.OrderBy(t => t).Distinct().Select(t=>new SettingItemModel()
                    {
                        Name = t,
                        Checked = true
                    }).ToList();
                }
                return null;
            });
        }

        public async Task<List<SettingItemModel>> GetWorkItemStatesAsync()
        {
            return await Task.Run(() =>
            {
                if (_context != null && _context.HasCollection && _context.HasTeamProject)
                {
                    var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();
                    var workItems = wis.Query("Select [State], [Title] " +
                                              "From WorkItems " +
                                              "Order By [State] Asc, [Changed Date] Desc");

                    var states = new List<String>();
                    foreach (WorkItem w in workItems)
                    {
                        if (!states.Contains(w.State))
                        {
                            states.Add(w.State);
                        }
                    }
                    return states.Select(s => new SettingItemModel()
                    {
                        Name = s,
                        Checked = false
                    }).ToList();
                };

                return new List<SettingItemModel>();
            });
        }
    }
}
