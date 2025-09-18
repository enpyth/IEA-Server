using api_demo.Models;
using api_demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfilesController> _logger;

        public ProfilesController(IProfileService profileService, ILogger<ProfilesController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        // GET: api/profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
        {
            try
            {
                var profiles = await _profileService.GetAllProfilesAsync();
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all profiles");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/profiles/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(Guid id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/profiles
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")] // Use custom policy for now
        public async Task<ActionResult<ProfileDto>> CreateProfile(CreateProfileDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var profile = await _profileService.CreateProfileAsync(createDto);
                return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/profiles/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")] // Use custom policy for now
        public async Task<IActionResult> UpdateProfile(Guid id, UpdateProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var profile = await _profileService.UpdateProfileAsync(id, updateDto);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/profiles/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")] // Use custom policy for now
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            try
            {
                var deleted = await _profileService.DeleteProfileAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
