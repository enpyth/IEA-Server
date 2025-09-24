using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Supabase;
using api_demo.Models;
using api_demo.Dtos.Collaboration;

namespace api_demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollaborationRequestsController : ControllerBase
    {
        private readonly Client _supabase;

        public CollaborationRequestsController(Client supabase)
        {
            _supabase = supabase;
        }

        /// <summary>
        /// Get all collaboration requests
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CollaborationRequestDto>), 200)]
        public async Task<ActionResult<IEnumerable<CollaborationRequestDto>>> GetCollaborationRequests()
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Get();

                var requests = response.Models.Select(r => new CollaborationRequestDto
                {
                    Id = r.Id,
                    SenderId = r.SenderId,
                    ReceiverId = r.ReceiverId,
                    Details = r.Details,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                });

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get collaboration request by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CollaborationRequestDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CollaborationRequestDto>> GetCollaborationRequest(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Where(r => r.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                var requestDto = new CollaborationRequestDto
                {
                    Id = response.Id,
                    SenderId = response.SenderId,
                    ReceiverId = response.ReceiverId,
                    Details = response.Details,
                    Status = response.Status,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new collaboration request
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CollaborationRequestDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CollaborationRequestDto>> CreateCollaborationRequest([FromBody] CreateCollaborationRequestDto createDto)
        {
            try
            {
                var request = new CollaborationRequest
                {
                    Id = Guid.NewGuid(),
                    SenderId = Guid.NewGuid(), // TODO: Get from authenticated user
                    ReceiverId = createDto.ReceiverId,
                    Details = createDto.Details,
                    Status = CollaborationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Insert(request);

                var createdRequest = response.Models.FirstOrDefault();
                if (createdRequest == null)
                    return BadRequest("Failed to create collaboration request");

                var requestDto = new CollaborationRequestDto
                {
                    Id = createdRequest.Id,
                    SenderId = createdRequest.SenderId,
                    ReceiverId = createdRequest.ReceiverId,
                    Details = createdRequest.Details,
                    Status = createdRequest.Status,
                    CreatedAt = createdRequest.CreatedAt,
                    UpdatedAt = createdRequest.UpdatedAt
                };

                return CreatedAtAction(nameof(GetCollaborationRequest), new { id = requestDto.Id }, requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Approve a collaboration request
        /// </summary>
        [HttpPost("{id}/approve")]
        [ProducesResponseType(typeof(CollaborationRequestDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CollaborationRequestDto>> ApproveCollaborationRequest(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Where(r => r.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                if (response.Status != CollaborationStatus.Pending)
                    return BadRequest("Only pending requests can be approved");

                response.Status = CollaborationStatus.Approved;
                response.UpdatedAt = DateTime.UtcNow;

                await response.Update<CollaborationRequest>();

                var requestDto = new CollaborationRequestDto
                {
                    Id = response.Id,
                    SenderId = response.SenderId,
                    ReceiverId = response.ReceiverId,
                    Details = response.Details,
                    Status = response.Status,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Reject a collaboration request
        /// </summary>
        [HttpPost("{id}/reject")]
        [ProducesResponseType(typeof(CollaborationRequestDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CollaborationRequestDto>> RejectCollaborationRequest(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Where(r => r.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                if (response.Status != CollaborationStatus.Pending)
                    return BadRequest("Only pending requests can be rejected");

                response.Status = CollaborationStatus.Rejected;
                response.UpdatedAt = DateTime.UtcNow;

                await response.Update<CollaborationRequest>();

                var requestDto = new CollaborationRequestDto
                {
                    Id = response.Id,
                    SenderId = response.SenderId,
                    ReceiverId = response.ReceiverId,
                    Details = response.Details,
                    Status = response.Status,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update collaboration request
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CollaborationRequestDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CollaborationRequestDto>> UpdateCollaborationRequest(Guid id, [FromBody] ReviewCollaborationRequestDto reviewDto)
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Where(r => r.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                response.Status = reviewDto.Status;
                response.UpdatedAt = DateTime.UtcNow;

                await response.Update<CollaborationRequest>();

                var requestDto = new CollaborationRequestDto
                {
                    Id = response.Id,
                    SenderId = response.SenderId,
                    ReceiverId = response.ReceiverId,
                    Details = response.Details,
                    Status = response.Status,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete collaboration request
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCollaborationRequest(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<CollaborationRequest>()
                    .Where(r => r.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                await response.Delete<CollaborationRequest>();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
