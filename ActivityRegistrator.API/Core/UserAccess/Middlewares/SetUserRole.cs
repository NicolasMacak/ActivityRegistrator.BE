using ActivityRegistrator.API.Service;
using Microsoft.Extensions.Primitives;

namespace ActivityRegistrator.API.Core.UserAccess.Middlewares;
public class SetUserRole
{
    private const string TenantCodeName = "x-tenant-code";
    private readonly RequestDelegate _next;

    public SetUserRole(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IActiveUserService activeUserService)
    {
        StringValues tenantCode = context.Request.Headers[TenantCodeName];
        string? email = context.User.GetEmail();

        if (!string.IsNullOrEmpty(tenantCode) && !string.IsNullOrEmpty(email))
        {
            activeUserService.SetUserProperties(tenantCode!, email!); 
        }

        await _next(context);
    }
}
