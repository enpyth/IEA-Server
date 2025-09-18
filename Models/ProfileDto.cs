using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class CreateProfileDto
    {
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        [EnumDataType(typeof(UserRole))]
        public UserRole? Role { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
    }
}
