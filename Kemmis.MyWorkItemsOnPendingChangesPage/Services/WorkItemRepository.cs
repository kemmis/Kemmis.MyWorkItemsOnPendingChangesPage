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
        private readonly ITeamFoundationContext _context;
        public WorkItemRepository(ITeamFoundationContext context)
        {
            _context = context;
        }

        public Task GetWorkItemTypesAsync(ObservableCollection<SettingItemModel> collection)
        {
            return Task.Run(() =>
            {
                if (_context != null && _context.HasCollection && _context.HasTeamProject)
                {
                    var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();

                    var types = new List<string>();

                    foreach (Project p in wis.Projects)
                    {
                        foreach (WorkItemType t in p.WorkItemTypes)
                        {
                            if (collection.All(s => s.Name != t.Name))
                            {
                                collection.Add(new SettingItemModel { Name = t.Name });
                            }
                        }
                    }
                }
            });
        }

        public Task GetWorkItemStatesAsync(ObservableCollection<SettingItemModel> collection)
        {
            return Task.Run(() =>
             {
                 if (_context != null && _context.HasCollection && _context.HasTeamProject)
                 {
                     var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();

                     var types = new List<string>();

                     foreach (Project p in wis.Projects)
                     {
                         foreach (WorkItemType t in p.WorkItemTypes)
                         {
                             foreach (string stateValue in t.FieldDefinitions["State"].AllowedValues)
                             {
                                 if (collection.All(s => s.Name != stateValue))
                                 {
                                     collection.Add(new SettingItemModel { Name = stateValue });
                                 }
                             }
                         }
                     }
                 }
             });
        }

        public Task GetWorkItemsAsync(ObservableCollection<WorkItemModel> collection, SettingsModel settings)
        {
            return Task.Run(() =>
            {
                if (_context != null && _context.HasCollection && _context.HasTeamProject)
                {
                    var sinceDate = DateTime.Now.AddDays(-settings.DaysBackToQuery).ToShortDateString();
                    var wis = _context.TeamProjectCollection.GetService<WorkItemStore>();
                    var states = settings.WorkItemStatuses.Where(w => w.Checked).Select(w => w.Name).ToArray();
                    var statesString = "'" + string.Join("','", states) + "'";
                    var types = settings.WorkItemTypes.Where(w => w.Checked).Select(w => w.Name).ToArray();
                    var typesString = "'" + string.Join("','", types) + "'";

                    var queryText = $@"select * from workitems where 
	                    [Changed Date] > '{sinceDate}' and 
	                    [State] in ({statesString}) and 
	                    [Assigned To]=@me and
	                    [Work Item Type] in ({typesString})
	                    order by [Changed Date] desc";

                    var workItems = wis.Query(queryText);

                    // clear collection so items truely get refreshed
                    collection.Clear();

                    foreach (WorkItem w in workItems)
                    {
                        if (collection.Count >= settings.MaxWorkItems)
                        {
                            break;
                        }

                        if (collection.All(t => t.Id != w.Id))
                        {
                            collection.Add(new WorkItemModel()
                            {
                                Title = w.Title,
                                Id = w.Id,
                                State = w.State,
                                WorkItemType = w.Type.Name,
                                AssignedTo = w["Assigned To"].ToString()
                            });
                        }
                    }

                };
            });
        }

        public Task GetColumnsAsync(ICollection<SettingItemModel> collection)
        {
            return Task.Run(() =>
            {
                collection.Add(new SettingItemModel
                {
                    Name = "Id",
                    Checked = true
                });

                collection.Add(new SettingItemModel
                {
                    Name = "Work Item Type",
                    Checked = true
                });

                collection.Add(new SettingItemModel
                {
                    Name = "Title",
                    Checked = true
                });

                collection.Add(new SettingItemModel
                {
                    Name = "State",
                    Checked = false
                });

                collection.Add(new SettingItemModel
                {
                    Name = "Assigned To",
                    Checked = false
                });
            });
        }
    }
}
