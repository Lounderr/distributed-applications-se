using System.ComponentModel.DataAnnotations;

using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Data.Models
{
    public abstract class BaseEntity : IIdentifiable, IAuditInfo, IDeletableEntity
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        [MaxLength(200)]
        public required string CreatedBy { get; set; }
    }
}