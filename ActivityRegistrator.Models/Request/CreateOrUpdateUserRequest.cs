using Azure;

namespace ActivityRegistrator.Models.Request;
public class CreateOrUpdateUserRequest
{
    public string TenantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    /// Null when creating
    /// </summary>
    public ETag? ETag { get; set; }
}
