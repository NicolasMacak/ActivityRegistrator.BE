using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.Models.ObjectResults;
[Obsolete("Do we really need this")]
public class PreconditionFailedObjectResult : ObjectResult
{
    public PreconditionFailedObjectResult(object? value) : base(value)
    {
        StatusCode = (int) HttpStatusCode.PreconditionFailed;
    }
}
