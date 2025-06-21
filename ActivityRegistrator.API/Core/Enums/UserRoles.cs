namespace ActivityRegistrator.API.Core.Enums;
public enum UserRoles
{
    /// <summary> Super admin, manages all Tenants </summary>
    Root = 0,
    /// <summary> Manages tenant data </summary>
    TenantAdmin = 1,
    /// <summary> Manages tenant data, but not all </summary>
    DelegatedAdmin = 2,
    /// <summary> Regular user, manages only their own data </summary>
    User = 3,
    /// <summary> Has not account. Access to non-tenant related data </summary>
    Guest = 4
}
