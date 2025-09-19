using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class Expert : User
    {
        [Required]
        [StringLength(200)]
        public string ExpertiseArea { get; set; } = string.Empty;

        public Expert()
        {
            Role = Role.Expert;
        }
    }
}


