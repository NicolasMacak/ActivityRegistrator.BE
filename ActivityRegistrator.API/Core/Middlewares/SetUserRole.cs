using ActivityRegistrator.API.Core.Enums;
using ActivityRegistrator.API.Service;


namespace ActivityRegistrator.API.Core.Middlewares;
public class SetUserRole
{
    private readonly RequestDelegate _next;

    public SetUserRole(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IActiveUserService activeUserService) // functionality not tested. Post some user with role to the db
    {
        var loggerUser = context.User; // stadeto vytiahnut email

        string? tenantCode = "TangoVida";// context.Request.Headers["Tenant-Code"].FirstOrDefault();
        string? email = "nicolas.macak@gmail.com";// context.Request.Headers["Email"].FirstOrDefault(); 
        if (!string.IsNullOrEmpty(tenantCode) && !string.IsNullOrEmpty(email))
        {
            await activeUserService.AssignUserRole(tenantCode, email);
        }
        else
        {
            activeUserService.DeclareUserAsGuest();
        }

        await _next(context);
    }
}
