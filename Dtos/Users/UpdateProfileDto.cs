using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.Users
{
    public class UpdateProfileDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        public Role? Role { get; set; }
    }
}



