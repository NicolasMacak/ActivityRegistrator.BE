using System.Security.Claims;
using ActivityRegistrator.API.Core.Enums;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;

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
        ActiveUserRole = await GetUserRole();
    }

    /// <summary>
    /// Returns role of the user. If user is not found or his role could not be parsed out, returns <see cref="UserRoles.Guest"/> role.
    /// </summary>
    private async Task<UserRoles> GetUserRole()
    {
        ResultWrapper<UserEntity> tenantUser = await _userService.GetAsync(TenantCode, Email);

        if (tenantUser.Status == OperationStatus.Success)
        {
            return Enum.TryParse(tenantUser.Value!.AccessRole, out UserRoles userRole) ?
                 userRole
                 : UserRoles.Guest;
        }

        return UserRoles.Guest;
    }
}
