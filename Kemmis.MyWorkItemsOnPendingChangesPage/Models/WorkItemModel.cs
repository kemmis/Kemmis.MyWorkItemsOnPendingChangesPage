using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Models
{
    public class WorkItemModel
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string WorkItemType { get; set; }
        public string State { get; set; }
        public string AssignedTo { get; set; }
    }
}
