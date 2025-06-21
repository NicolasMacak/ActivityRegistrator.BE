using System.Security.Claims;

namespace ActivityRegistrator.API.Service;
public interface IActiveUserService
{
    string TenantCode { get; }
    public string Email { get; }

    /// <summary>
    /// 
    /// </summary>
    public void SetUserProperties(string tenantCode, string email);
}
