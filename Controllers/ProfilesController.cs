using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Supabase;
using api_demo.Models;
using ProfileDto = api_demo.Dtos.Users.ProfileDto;
using CreateProfileDto = api_demo.Dtos.Users.CreateProfileDto;
using UpdateProfileRoleDto = api_demo.Dtos.Users.UpdateProfileRoleDto;

namespace api_demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly Client _supabase;

        public ProfilesController(Client supabase)
        {
            _supabase = supabase;
        }

        /// <summary>
        /// Get all profiles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProfileDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
        {
            try
            {
                var response = await _supabase
                    .From<Profiles>()
                    .Get();

                var profiles = response.Models.Select(p => new ProfileDto
                {
                    Id = p.Id,
                    Email = p.Email,
                    Role = p.Role,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                });

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get profile by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProfileDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProfileDto>> GetProfile(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<Profiles>()
                    .Where(p => p.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                var profileDto = new ProfileDto
                {
                    Id = response.Id,
                    Email = response.Email,
                    Role = response.Role,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new profile
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProfileDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProfileDto>> CreateProfile([FromBody] CreateProfileDto createProfileDto)
        {
            try
            {
                var profile = new Profiles
                {
                    Id = Guid.NewGuid(),
                    Email = createProfileDto.Email,
                    Role = createProfileDto.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var response = await _supabase
                    .From<Profiles>()
                    .Insert(profile);

                var createdProfile = response.Models.FirstOrDefault();
                if (createdProfile == null)
                    return BadRequest("Failed to create profile");

                var profileDto = new ProfileDto
                {
                    Id = createdProfile.Id,
                    Email = createdProfile.Email,
                    Role = createdProfile.Role,
                    CreatedAt = createdProfile.CreatedAt,
                    UpdatedAt = createdProfile.UpdatedAt
                };

                return CreatedAtAction(nameof(GetProfile), new { id = profileDto.Id }, profileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update profile role
        /// </summary>
        [HttpPut("{id}/role")]
        [ProducesResponseType(typeof(ProfileDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProfileDto>> UpdateProfileRole(Guid id, [FromBody] UpdateProfileRoleDto updateRoleDto)
        {
            try
            {
                var response = await _supabase
                    .From<Profiles>()
                    .Where(p => p.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                response.Role = updateRoleDto.Role;
                response.UpdatedAt = DateTime.UtcNow;

                await response.Update<Profiles>();

                var profileDto = new ProfileDto
                {
                    Id = response.Id,
                    Email = response.Email,
                    Role = response.Role,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete profile
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            try
            {
                var response = await _supabase
                    .From<Profiles>()
                    .Where(p => p.Id == id)
                    .Single();

                if (response == null)
                    return NotFound();

                await response.Delete<Profiles>();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}