using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.Domain
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "ServiceName is a mandatory field")]
        public string? ServiceName { get; set; }

        // Foreign key for User
        public int UserId { get; set; }

        // Navigation property for User
        public User User { get; set; }

        public bool IsDeleted { get; set; }
    }
}
