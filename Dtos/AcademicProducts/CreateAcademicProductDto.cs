using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_demo.Dtos.AcademicProducts
{
    /// <summary>
    /// DTO for creating a new academic product. ExpertId is automatically set from the authenticated user.
    /// </summary>
    /// <example>
    /// {
    ///   "achievements": {
    ///     "publications": 5,
    ///     "awards": ["Best Paper 2023", "Innovation Award"],
    ///     "hIndex": 15
    ///   },
    ///   "title": "Advanced Machine Learning Research",
    ///   "description": "Comprehensive research on deep learning algorithms"
    /// }
    /// </example>
    public class CreateAcademicProductDto
    {
        /// <summary>
        /// Expert's achievements and metrics (JSON object)
        /// </summary>
        [JsonPropertyName("achievements")]
        public Dictionary<string, object> Achievements { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Title of the academic product
        /// </summary>
        [Required]
        [StringLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description of the academic product
        /// </summary>
        [StringLength(2000)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}


