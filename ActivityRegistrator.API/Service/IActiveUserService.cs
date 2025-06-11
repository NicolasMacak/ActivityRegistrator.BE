using ActivityRegistrator.API.Core.Enums;

namespace ActivityRegistrator.API.Service;
public interface IActiveUserService
{
    public UserRoles ActiveUserRole { get; }

    public Task AssignUserRole(string tenantCode, string email);

    public void DeclareUserAsGuest();
}
