using System.Security.Claims;
using ActivityRegistrator.API.Core.Enums;

namespace ActivityRegistrator.API.Service;
public class ActiveUserService : IActiveUserService
{
    private const string EmailKeyInClams = "emails";

    private readonly IUserService _userService;

    public ActiveUserService(IUserService userService)
    {
        _userService = userService;
    }

    public string TenantCode { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public UserRoles ActiveUserRole { get; private set; } = UserRoles.Guest;

    public async Task SetUserProperties(string tenantCode, IEnumerable<Claim> claims)
    {
        TenantCode = tenantCode;
        Email = claims.FirstOrDefault(c => c.Type == EmailKeyInClams)?.Value ?? string.Empty;
        ActiveUserRole = await _userService.GetUserRole(TenantCode, Email);
    }
}
