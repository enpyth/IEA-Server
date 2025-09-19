using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.AcademicProducts
{
    public class AcademicProductDto
    {
        public Guid Id { get; set; }
        public Guid ExpertId { get; set; }
        public Dictionary<string, object> Achievements { get; set; } = new Dictionary<string, object>();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


