using System.Security.Claims;

public static class ClaimsPrincialExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
         string EmailKeyInClams = "emails";
        return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == EmailKeyInClams)?.Value ?? string.Empty;

    }
}
