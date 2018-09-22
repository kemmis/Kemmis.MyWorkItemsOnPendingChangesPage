using System;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Models
{
    public class SettingItemModel : IComparable<SettingItemModel>
    {
        public bool Checked { get; set; }

        public string Name { get; set; }

        public int CompareTo(SettingItemModel other)
        {
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}