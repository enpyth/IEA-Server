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
                if (profile != null && (profile.Role == Models.Role.Admin)) 
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

    public class ExpertRequirement : IAuthorizationRequirement
    {
        public ExpertRequirement() { }
    }

    public class ExpertAuthorizationHandler : AuthorizationHandler<ExpertRequirement>
    {
        private readonly IProfileService _profileService;

        public ExpertAuthorizationHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ExpertRequirement requirement)
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
                if (profile != null && (profile.Role == Models.Role.Expert || profile.Role == Models.Role.Admin))
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

    public class EnterpriseRequirement : IAuthorizationRequirement
    {
        public EnterpriseRequirement() { }
    }

    public class EnterpriseAuthorizationHandler : AuthorizationHandler<EnterpriseRequirement>
    {
        private readonly IProfileService _profileService;

        public EnterpriseAuthorizationHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EnterpriseRequirement requirement)
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
                if (profile != null && (profile.Role == Models.Role.Enterprise || profile.Role == Models.Role.Admin))
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
