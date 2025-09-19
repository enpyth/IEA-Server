using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.Collaboration
{
    public class ReviewCollaborationRequestDto
    {
        [Required]
        public CollaborationStatus Status { get; set; }

        [StringLength(500)]
        public string? ReviewerComment { get; set; }
    }
}


