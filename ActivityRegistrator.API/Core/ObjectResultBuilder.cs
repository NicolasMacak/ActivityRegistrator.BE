using ActivityRegistrator.Models.ObjectResults;

namespace ActivityRegistrator.API.Core;
public static class ObjectResultBuilder
{
    public static PreconditionFailedObjectResult PreconditionFailed(object value)
    {
        return new PreconditionFailedObjectResult(value);
    }
}
