using Azure;

namespace ActivityRegistrator.Models.Request;
public class UpdateUserRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public ETag ETag { get; set; }
}
