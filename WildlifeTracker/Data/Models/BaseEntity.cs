using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Data.Models
{
    public class BaseEntity : IIdentifiable, IAuditInfo
    {
        public int Id { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}