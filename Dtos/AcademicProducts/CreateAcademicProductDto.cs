using System.ComponentModel.DataAnnotations;

namespace api_demo.Dtos.AcademicProducts
{
    public class CreateAcademicProductDto
    {
        [Required]
        public Guid ExpertId { get; set; }

        public Dictionary<string, object> Achievements { get; set; } = new Dictionary<string, object>();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}


