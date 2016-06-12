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
        public ObservableCollection<SettingItemModel> WorkItemTypes { get; set; }
        public ObservableCollection<SettingItemModel> WorkItemStatuses { get; set; }
        public int DaysBackToQuery { get; set; }
    }
}
