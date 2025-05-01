namespace WildlifeTracker.Data.Models.Interfaces
{
    internal interface IAuditInfo
    {
        DateTime CreatedOn { get; set; }
        DateTime? ModifiedOn { get; set; }
    }
}