using ActivityRegistrator.API.Core.DataProcessing.Enums;
namespace ActivityRegistrator.API.Core.DataProcessing.Model;
/// <summary>
/// May contain Value of type <see cref="T"/> and <see cref="OperationStatus"/> providing information about operation execution
/// </summary>
/// <typeparam name="T"></typeparam>
public class ServiceResult<T>
{
    public IDto? Value { get; private set; }
    public OperationStatus Status { get; private set; }

    public ServiceResult<T> With(IDto value, OperationStatus status)
    {
        Value = value;
        Status = status;

        return this;
    }

    public ServiceResult<T> With(IDto value)
    {
        Value = value;
        Status = OperationStatus.Success;

        return this;
    }

    public ServiceResult<T> With(OperationStatus status)
    {
        Status = status;

        return this;
    }
}
