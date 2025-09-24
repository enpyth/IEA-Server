using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("experts")]
    public class Expert : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("expertise_area")]
        [Required]
        [StringLength(200)]
        public string ExpertiseArea { get; set; } = string.Empty;
    }
}


