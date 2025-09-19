using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class CollaborationRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public CollaborationStatus Status { get; set; } = CollaborationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }
}


