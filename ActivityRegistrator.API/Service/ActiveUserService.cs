using System.Security.Claims;
namespace ActivityRegistrator.API.Service;
public class ActiveUserService : IActiveUserService
{
    public string TenantCode { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public void SetUserProperties(string tenantCode, string email)
    {
        TenantCode = tenantCode;
        Email = email;
    }
}
