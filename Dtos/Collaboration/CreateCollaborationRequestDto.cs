using System.ComponentModel.DataAnnotations;

namespace api_demo.Dtos.Collaboration
{
    public class CreateCollaborationRequestDto
    {
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Details { get; set; } = string.Empty;
    }
}


