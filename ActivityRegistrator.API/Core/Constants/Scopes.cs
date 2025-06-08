namespace ActivityRegistrator.API.Core.Constants;
/// <summary>
/// Authorized access comes through Web Api reigstration. There, defined scopes differenent kind of access to the api
/// </summary>
public static class Scopes
{
    public const string Read = "api.read";
    public const string Write = "api.write";
}
