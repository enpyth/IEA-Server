using api_demo.Models;
using api_demo.Dtos.Users;
using api_demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api_demo.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<MeController> _logger;

        public MeController(IProfileService profileService, ILogger<MeController> logger)
        {
            _profileService = profileService;
            _logger = logger;
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

        // GET: api/me/profile
        [HttpGet("profile")]
        public async Task<ActionResult<Profiles>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _profileService.GetCurrentUserProfileAsync(userId);
                
                if (profile == null)
                {
                    return NotFound("Profile not found");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user profile");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/me/profile
        [HttpPut("profile")]
        public async Task<ActionResult<Profiles>> UpdateMyProfile(UpdateProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var profile = await _profileService.UpdateCurrentUserProfileAsync(userId, updateDto);
                
                if (profile == null)
                {
                    return NotFound("Profile not found");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current user profile");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
