using System.Text.Json.Serialization;

namespace api_demo.Models
{
    public static class ProfileExtensions
    {
        public static ProfileDto ToDto(this Profile profile)
        {
            return new ProfileDto
            {
                Id = profile.Id,
                Role = profile.Role,
                Email = profile.Email
            };
        }
    }
}
