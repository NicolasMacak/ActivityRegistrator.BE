using ActivityRegistrator.API.Core.DataProcessing.Model;

namespace ActivityRegistrator.API.Core.DataProcessing.Builders;
public static class ObjectResultBuilder
{
    public static PreconditionFailedObjectResult PreconditionFailed(object value)
    {
        return new PreconditionFailedObjectResult(value);
    }
}
