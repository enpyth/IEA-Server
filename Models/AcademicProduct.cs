using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class AcademicProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ExpertId { get; set; }

        // JSONB-like structure; concrete mapping configured in DbContext later
        public Dictionary<string, object> Achievements { get; set; } = new Dictionary<string, object>();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Expert? Expert { get; set; }
    }
}


