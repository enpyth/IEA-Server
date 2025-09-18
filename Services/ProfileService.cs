using api_demo.Models;
using Supabase.Postgrest;

namespace api_demo.Services
{
    public class ProfileService : IProfileService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(Supabase.Client supabaseClient, ILogger<ProfileService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ProfileDto>> GetAllProfilesAsync()
        {
            try
            {
                var response = await _supabaseClient
                    .From<Profile>()
                    .Get();
                
                return response.Models?.Select(p => p.ToDto()) ?? Enumerable.Empty<ProfileDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all profiles");
                throw;
            }
        }

        public async Task<ProfileDto?> GetProfileByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabaseClient
                    .From<Profile>()
                    .Filter("id", Constants.Operator.Equals, id.ToString())
                    .Get();
                
                var profile = response.Models?.FirstOrDefault();
                return profile?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile by id: {Id}", id);
                throw;
            }
        }

        public async Task<ProfileDto> CreateProfileAsync(CreateProfileDto createDto)
        {
            try
            {
                var profile = new Profile
                {
                    Id = Guid.NewGuid(),
                    Role = createDto.Role,
                    Email = createDto.Email
                };

                var response = await _supabaseClient
                    .From<Profile>()
                    .Insert(profile);
                
                var createdProfile = response.Models?.FirstOrDefault();
                if (createdProfile == null)
                {
                    throw new InvalidOperationException("Failed to create profile");
                }
                
                return createdProfile.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                throw;
            }
        }

        public async Task<ProfileDto?> UpdateProfileAsync(Guid id, UpdateProfileDto updateDto)
        {
            try
            {
                if (!updateDto.Role.HasValue && string.IsNullOrEmpty(updateDto.Email))
                {
                    return await GetProfileByIdAsync(id);
                }

                var profile = await GetProfileByIdAsync(id);
                if (profile == null)
                {
                    return null;
                }

                var updateProfile = new Profile
                {
                    Id = id,
                    Role = updateDto.Role ?? profile.Role,
                    Email = updateDto.Email ?? profile.Email
                };

                await _supabaseClient
                    .From<Profile>()
                    .Filter("id", Constants.Operator.Equals, id.ToString())
                    .Update(updateProfile);
                
                return await GetProfileByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProfileAsync(Guid id)
        {
            try
            {
                await _supabaseClient
                    .From<Profile>()
                    .Filter("id", Constants.Operator.Equals, id.ToString())
                    .Delete();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile: {Id}", id);
                throw;
            }
        }

        public async Task<ProfileDto?> GetCurrentUserProfileAsync(string userId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<Profile>()
                    .Filter("id", Constants.Operator.Equals, userId)
                    .Get();
                
                var profile = response.Models?.FirstOrDefault();
                return profile?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user profile: {UserId}", userId);
                throw;
            }
        }

        public async Task<ProfileDto?> UpdateCurrentUserProfileAsync(string userId, UpdateProfileDto updateDto)
        {
            return await UpdateProfileAsync(Guid.Parse(userId), updateDto);
        }
    }
}
