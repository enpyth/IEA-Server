using System.Text.Json.Serialization;
using api_demo.Dtos.Users;

namespace api_demo.Models
{
    public static class ProfileExtensions
    {
        public static ProfileDto ToDto(this Profiles profile)
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
