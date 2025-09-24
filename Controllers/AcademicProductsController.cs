using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Supabase;
using api_demo.Models;
using api_demo.Dtos.AcademicProducts;
using api_demo.Services;
using System.Security.Claims;

namespace api_demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AcademicProductsController : ControllerBase
    {
        private readonly Client _supabase;
        private readonly IProfileService _profileService;
        private readonly IAcademicProductService _academicProductService;

        public AcademicProductsController(Client supabase, IProfileService profileService, IAcademicProductService academicProductService)
        {
            _supabase = supabase;
            _profileService = profileService;
            _academicProductService = academicProductService;
        }

        private string GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID not found in claims");
            }
            
            return userId;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AcademicProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<AcademicProductDto>>> GetAcademicProducts()
        {
            try
            {
                var products = await _academicProductService.GetAllAcademicProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AcademicProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AcademicProductDto>> GetAcademicProduct(Guid id)
        {
            try
            {
                var product = await _academicProductService.GetAcademicProductByIdAsync(id);
                
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("expert/{expertId}")]
        [ProducesResponseType(typeof(IEnumerable<AcademicProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<AcademicProductDto>>> GetAcademicProductsByExpert(Guid expertId)
        {
            try
            {
                var products = await _academicProductService.GetAcademicProductsByExpertAsync(expertId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AcademicProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = "ExpertPolicy")]
        public async Task<ActionResult<AcademicProductDto>> CreateAcademicProduct([FromBody] CreateAcademicProductDto createDto)
        {
            try
            {
                // Get current user ID and use it as ExpertId
                var currentUserId = GetCurrentUserId();
                
                // Log auth.uid() and expert_id for debugging
                Console.WriteLine($"[POST AcademicProduct] auth.uid(): {currentUserId}");
                Console.WriteLine($"[POST AcademicProduct] expert_id: {currentUserId}");
                
                // First verify that the user has 'expert' role in profiles table
                var userIdGuid = Guid.Parse(currentUserId);
                var profileResponse = await _supabase
                    .From<Profiles>()
                    .Where(p => p.Id == userIdGuid)
                    .Single();
                
                if (profileResponse == null)
                {
                    Console.WriteLine($"[POST AcademicProduct] User {currentUserId} profile not found");
                    return StatusCode(403, "User profile not found");
                }
                
                Console.WriteLine($"[POST AcademicProduct] Found profile: {profileResponse.Email}, Role: {profileResponse.Role}");
                
                if (profileResponse.Role != Role.Expert)
                {
                    Console.WriteLine($"[POST AcademicProduct] User {currentUserId} role is {profileResponse.Role}, not Expert");
                    return StatusCode(403, $"Only experts can create academic products. Current role: {profileResponse.Role}");
                }
                
                var productDto = await _academicProductService.CreateAcademicProductAsync(createDto, userIdGuid);
                return CreatedAtAction(nameof(GetAcademicProduct), new { id = productDto.Id }, productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AcademicProductDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "ExpertPolicy")]
        public async Task<ActionResult<AcademicProductDto>> UpdateAcademicProduct(Guid id, [FromBody] UpdateAcademicProductDto updateDto)
        {
            try
            {
                var productDto = await _academicProductService.UpdateAcademicProductAsync(id, updateDto);
                
                if (productDto == null)
                    return NotFound();

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "ExpertPolicy")]
        public async Task<IActionResult> DeleteAcademicProduct(Guid id)
        {
            try
            {
                var deleted = await _academicProductService.DeleteAcademicProductAsync(id);
                
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
