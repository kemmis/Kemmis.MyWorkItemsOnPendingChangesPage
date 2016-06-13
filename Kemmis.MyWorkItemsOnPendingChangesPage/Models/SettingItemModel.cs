using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Models
{
    public class SettingItemModel
    {
        private string _name;
        public bool Checked { get; set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
