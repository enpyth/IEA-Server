using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace api_demo.Models
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Column("role")]
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
        
        [Column("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public enum UserRole
    {
        Expert,
        Visitor,
        Enterprise
    }
}
