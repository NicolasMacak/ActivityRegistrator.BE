using ActivityRegistrator.API.Core.Enums;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;

namespace ActivityRegistrator.API.Service;
public class ActiveUserService : IActiveUserService
{
    private readonly IUserService _userService;
    public ActiveUserService(IUserService userService)
    {
        _userService = userService;
    }

    public UserRoles ActiveUserRole { get; private set; }

    public async Task AssignUserRole(string tenantCode, string email)
    {
        ResultWrapper<UserEntity> tenantUser = await _userService.GetAsync(tenantCode, email);

        if (tenantUser.Status == OperationStatus.Success) // Failure or not found
        {
            ActiveUserRole = (UserRoles) tenantUser.Value!.UserRole;
            return;
        }

        ActiveUserRole = UserRoles.Guest;
    }

    public void DeclareUserAsGuest()
    {
        ActiveUserRole = UserRoles.Guest;
    }
}
