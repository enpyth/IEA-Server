using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.Users
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }
    }
}


