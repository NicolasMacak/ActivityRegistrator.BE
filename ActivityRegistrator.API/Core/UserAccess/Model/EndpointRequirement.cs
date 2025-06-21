using ActivityRegistrator.API.Core.UserAccess.Enums;
using Microsoft.AspNetCore.Authorization;

public class EndpointRequirement : IAuthorizationRequirement
{
    public UserAccessLevel RequiredRole { get; } = UserAccessLevel.Guest;

    public EndpointRequirement(UserAccessLevel requiredRole)
    {
        RequiredRole = requiredRole;
    }
}
