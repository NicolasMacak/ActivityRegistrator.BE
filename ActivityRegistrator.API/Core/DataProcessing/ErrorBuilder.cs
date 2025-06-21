namespace ActivityRegistrator.API.Core.DataProcessing;
public static class ErrorBuilder
{
    public static Dictionary<string, object> NotFoundError(Dictionary<string, object> parameters)
    {
        return new Dictionary<string, object>()
        {
            { "message", "Resource not found" },
            { "parameters", parameters }
        };
    }

    public static Dictionary<string, object> AlreadyExistsError(Dictionary<string, object> parameters)
    {
        return new Dictionary<string, object>()
        {
            { "message", "Such resource already exists" },
            { "parameters", parameters }
        };
    }

    public static Dictionary<string, object> AlreadyUpdatedError(Dictionary<string, object> parameters)
    {
        return new Dictionary<string, object>()
        {
            { "message", "Concurency error. Related resource was already updated." },
            { "parameters", parameters }
        };
    }
}
