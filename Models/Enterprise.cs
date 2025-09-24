using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("enterprises")]
    public class Enterprise : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("company_name")]
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;
    }
}


