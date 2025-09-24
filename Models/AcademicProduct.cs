using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("academic_products")]
    public class AcademicProduct : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("expert_id")]
        [Required]
        public Guid ExpertId { get; set; }

        [Column("achievements")]
        // JSONB structure for PostgreSQL
        public Dictionary<string, object> Achievements { get; set; } = new Dictionary<string, object>();

        [Column("title")]
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        [StringLength(2000)]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


