using api_demo.Models;
using api_demo.Dtos.Users;

namespace api_demo.Services
{
    public interface IProfileService
    {
        Task<IEnumerable<ProfileDto>> GetAllProfilesAsync();
        Task<ProfileDto?> GetProfileByIdAsync(Guid id);
        Task<ProfileDto> CreateProfileAsync(CreateProfileDto createDto);
        Task<ProfileDto?> UpdateProfileAsync(Guid id, UpdateProfileDto updateDto);
        Task<bool> DeleteProfileAsync(Guid id);
        Task<ProfileDto?> GetCurrentUserProfileAsync(string userId);
        Task<ProfileDto?> UpdateCurrentUserProfileAsync(string userId, UpdateProfileDto updateDto);
    }
}
