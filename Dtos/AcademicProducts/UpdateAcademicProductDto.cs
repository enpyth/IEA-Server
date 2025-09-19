using System.ComponentModel.DataAnnotations;

namespace api_demo.Dtos.AcademicProducts
{
    public class UpdateAcademicProductDto
    {
        public Dictionary<string, object>? Achievements { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}


