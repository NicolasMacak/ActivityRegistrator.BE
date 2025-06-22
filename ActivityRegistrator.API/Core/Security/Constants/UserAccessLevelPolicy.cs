namespace ActivityRegistrator.API.Core.Security.Constants;
/// <summary>
/// Authorized access comes through Web Api reigstration. There, defined scopes differenent kind of access to the api
/// </summary>
public static class UserAccessLevelPolicy
{
    public const string RootAccess = "RootAccess";
    public const string TenantAdminAccess = "TenantAdminAccess";
    public const string DelegatedTenantAdminAccess = "DelegatedTenantAdminAccess";
    public const string UserAccess = "UserAccess";
    public const string GuestAccess = "GuestAccess";
}
