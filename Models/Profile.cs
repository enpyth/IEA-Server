using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("profiles")]
    public class Profiles : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Column("role")]
        [Required]
        public Role Role { get; set; } = Role.Member;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
