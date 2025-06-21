using Azure;

namespace ActivityRegistrator.Models.Dtoes;
public class UserDto
{   
    public string TenantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public ETag ETag { get; set; }
}
