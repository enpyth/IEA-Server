using System.ComponentModel.DataAnnotations;
using api_demo.Models;

namespace api_demo.Dtos.Collaboration
{
    public class CollaborationRequestDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Details { get; set; } = string.Empty;
        public CollaborationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


