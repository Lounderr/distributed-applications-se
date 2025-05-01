using System.Text.Json.Serialization;

using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Data.Models
{
    public class BaseEntity : IIdentifiable, IAuditInfo, IDeletableEntity
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime? ModifiedOn { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public DateTime? DeletedOn { get; set; }
    }
}