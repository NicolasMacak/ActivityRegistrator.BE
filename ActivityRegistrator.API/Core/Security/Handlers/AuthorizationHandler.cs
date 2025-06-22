using ActivityRegistrator.API.Core.Security.Enums;
using ActivityRegistrator.API.Service;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace ActivityRegistrator.API.Core.Security.Handlers
{
    public class EndpointAccessHandler : AuthorizationHandler<EndpointRequirement> //todo. Token lifetime set to 6 hours. Change after developement
    {
        private const string TenantCodeHeaderName = "x-tenant-code";

        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EndpointAccessHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EndpointRequirement requirement)
        {
            if (context.User == null || !context.User.Identity!.IsAuthenticated) // User not authenticated
            {
                return; // 401
            }

            StringValues tenantCode = _httpContextAccessor!.HttpContext!.Request.Headers[TenantCodeHeaderName];
            string? userEmail = context.User.GetEmail();

            if (string.IsNullOrEmpty(tenantCode) || string.IsNullOrEmpty(userEmail)) // Missing properties to declare authorization level
            {
                return; // 403
            }

            UserAccessLevel userAcessLevel = await GetUserAccessLevel(tenantCode!, userEmail);
            UserAccessLevel requiredLevel = requirement.RequiredRole;

            if(userAcessLevel >= requiredLevel) // Does the user access level of user suffice
            {
                context.Succeed(requirement);
            }

            return; // 406
        }

        private async Task<UserAccessLevel> GetUserAccessLevel(string tenantCode, string email)
        {
            ResultWrapper<UserEntity> tenantUser = await _userService.GetAsync(tenantCode, email);

            if (tenantUser.Status == OperationStatus.Success)
            {
                return Enum.TryParse(tenantUser.Value!.AccessLevel, out UserAccessLevel userAccessLevel) ?
                     userAccessLevel
                     : UserAccessLevel.Guest;
            }

            return UserAccessLevel.Guest;
        }
    }
}
