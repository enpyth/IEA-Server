using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.Users
{
    public class UpdateProfileRoleDto
    {
        [Required]
        public Role Role { get; set; }
    }
}


