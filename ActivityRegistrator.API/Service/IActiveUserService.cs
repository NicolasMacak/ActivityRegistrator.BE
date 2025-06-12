using System.Security.Claims;
using ActivityRegistrator.API.Core.Enums;

namespace ActivityRegistrator.API.Service;
public interface IActiveUserService
{
     string TenantCode { get; }
    public string Email { get; }
    public UserRoles ActiveUserRole { get; }

    public Task SetUserProperties(string tenantCode, IEnumerable<Claim> claims);
}
