using System.Security.Claims;
using ActivityRegistrator.API.Core.Enums;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;

namespace ActivityRegistrator.API.Service;
public class ActiveUserService : IActiveUserService
{
    private const string EmailKeyInClams = "emails";

    private readonly ILogger<ActiveUserService> _logger;
    private readonly IUserService _userService;

    public ActiveUserService(ILogger<ActiveUserService> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    public string TenantCode { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public UserRoles ActiveUserRole { get; private set; } = UserRoles.Guest;

    public async Task SetUserProperties(string tenantCode, IEnumerable<Claim> claims)
    {
        TenantCode = tenantCode;
        Email = claims.FirstOrDefault(c => c.Type == EmailKeyInClams)?.Value ?? string.Empty;
        ActiveUserRole = await GetUserRole();
    }

    private async Task<UserRoles> GetUserRole()
    {
        ResultWrapper<UserEntity> tenantUser = await _userService.GetAsync(TenantCode, Email);

        if (tenantUser.Status == OperationStatus.Success) // Failure or not found
        {
            return (UserRoles) tenantUser.Value!.AccessRole;
        }

        return UserRoles.Guest;
    }
}
