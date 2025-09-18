using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace api_demo.Services
{
    public class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement() { }
    }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly IProfileService _profileService;

        public AdminAuthorizationHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            AdminRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? context.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            try
            {
                var profile = await _profileService.GetCurrentUserProfileAsync(userId);
                if (profile != null && (profile.Role == Models.UserRole.Visitor)) // Both Expert and Enterprise can be admin
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            catch
            {
                context.Fail();
            }
        }
    }
}
