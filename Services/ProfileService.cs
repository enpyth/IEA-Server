using api_demo.Models;
using api_demo.Dtos.Users;
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
                    .From<Profiles>()
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
                    .From<Profiles>()
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
                var profile = new Profiles
                {
                    Id = Guid.NewGuid(),
                    Role = createDto.Role,
                    Email = createDto.Email
                };

                var response = await _supabaseClient
                    .From<Profiles>()
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

                var updateProfile = new Profiles
                {
                    Id = id,
                    Role = updateDto.Role ?? profile.Role,
                    Email = updateDto.Email ?? profile.Email
                };

                await _supabaseClient
                    .From<Profiles>()
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
                    .From<Profiles>()
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
                // Convert string userId to Guid for proper database query
                var userIdGuid = Guid.Parse(userId);
                Console.WriteLine($"[ProfileService] Looking for user with ID: {userIdGuid}");
                
                var response = await _supabaseClient
                    .From<Profiles>()
                    .Where(p => p.Id == userIdGuid)
                    .Get();
                
                Console.WriteLine($"[ProfileService] Found {response.Models?.Count ?? 0} profiles");
                
                var profile = response.Models?.FirstOrDefault();
                if (profile != null)
                {
                    Console.WriteLine($"[ProfileService] Found profile: {profile.Email}, Role: {profile.Role}");
                }
                else
                {
                    Console.WriteLine($"[ProfileService] No profile found for user {userIdGuid}");
                }
                
                return profile?.ToDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileService] Exception: {ex.Message}");
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
