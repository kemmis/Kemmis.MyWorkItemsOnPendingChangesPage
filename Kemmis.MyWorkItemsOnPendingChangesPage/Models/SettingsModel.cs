using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Models
{
    public class SettingsModel
    {
        public List<SettingItemModel> WorkItemTypes { get; set; }
        public List<SettingItemModel> WorkItemStatuses { get; set; }
        public List<SettingItemModel> Columns { get; set; } = new List<SettingItemModel>();
        public int DaysBackToQuery { get; set; }
        public int MaxWorkItems { get; set; }
    }
}
