namespace Kemmis.MyWorkItemsOnPendingChangesPage.Models
{
    public class WorkItemModel
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string WorkItemType { get; set; }
        public string State { get; set; }
        public string AssignedTo { get; set; }
        public string IdAndTitle => Id + " - " + Title;
    }
}