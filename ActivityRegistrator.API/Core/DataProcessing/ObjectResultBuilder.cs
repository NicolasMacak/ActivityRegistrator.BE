namespace ActivityRegistrator.API.Core.DataProcessing;
public static class ObjectResultBuilder
{
    public static PreconditionFailedObjectResult PreconditionFailed(object value)
    {
        return new PreconditionFailedObjectResult(value);
    }
}
