namespace ActivityRegistrator.API.Core.Enums;
public enum UserRoles
{
    Root = 0, // Super admin, manages all Tenants
    TenantAdmin = 1, // Manages tenant data
    DelegatedAdmin = 2, // Manages tenant data, but not all
    User = 3, // Regular user, manages only their own data
    Guest = 4 // Has no account
}
