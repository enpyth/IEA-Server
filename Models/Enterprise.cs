using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class Enterprise : User
    {
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public Enterprise()
        {
            Role = Role.Enterprise;
        }
    }
}


