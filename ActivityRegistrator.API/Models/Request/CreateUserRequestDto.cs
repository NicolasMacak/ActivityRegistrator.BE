namespace ActivityRegistrator.Models.Request;
public class CreateUserRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
