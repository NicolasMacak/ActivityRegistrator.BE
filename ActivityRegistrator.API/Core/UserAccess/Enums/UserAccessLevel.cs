namespace ActivityRegistrator.API.Core.UserAccess.Enums;
public enum UserAccessLevel
{
    /// <summary> Super admin, manages all Tenants </summary>
    Root = 4,
    /// <summary> Manages tenant data </summary>
    TenantAdmin = 3,
    /// <summary> Manages tenant data, but not all </summary>
    DelegatedAdmin = 2,
    /// <summary> Regular user, manages only their own data </summary>
    User = 1,
    /// <summary> Has not account. Access to non-tenant related data </summary>
    Guest = 0
}
