namespace ActivityRegistrator.API.Core;
public static class ErrorBuilder
{
    public static Dictionary<string, string> NotFoundError(Dictionary<string, string> parameters)
    {
        return new Dictionary<string, string>(parameters)
        {
            { "message", "Resource not found" }
        };
    }
}
