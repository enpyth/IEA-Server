using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("collaboration_requests")]
    public class CollaborationRequest : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("sender_id")]
        [Required]
        public Guid SenderId { get; set; }

        [Column("receiver_id")]
        [Required]
        public Guid ReceiverId { get; set; }

        [Column("details")]
        [Required]
        [StringLength(1000)]
        public string Details { get; set; } = string.Empty;

        [Column("status")]
        [Required]
        public CollaborationStatus Status { get; set; } = CollaborationStatus.Pending;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


