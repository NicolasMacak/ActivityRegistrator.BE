using ActivityRegistrator.API.Service;
using Microsoft.Extensions.Primitives;

namespace ActivityRegistrator.API.Middlewares;
public class SetUserRole
{
    private const string TenantCodeName = "x-tenant-code";
    private readonly RequestDelegate _next;

    public SetUserRole(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IActiveUserService activeUserService) // functionality not tested. Post some user with role to the db
    {
        StringValues tenantCode = context.Request.Headers[TenantCodeName];

        if (!string.IsNullOrEmpty(tenantCode))
        {
            await activeUserService.SetUserProperties(tenantCode!, context.User.Claims); 
        }

        await _next(context);
    }
}
