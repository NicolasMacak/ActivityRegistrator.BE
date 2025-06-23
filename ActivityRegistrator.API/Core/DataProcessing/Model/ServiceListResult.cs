using ActivityRegistrator.API.Core.DataProcessing.Enums;

namespace ActivityRegistrator.API.Core.DataProcessing.Model;
/// <summary>
/// May contain Values of type <see cref="T"/> and <see cref="OperationStatus"/> providing information about operation execution
/// </summary>
/// <typeparam name="T"></typeparam>
public class ServiceListResult<T>
{
    public IEnumerable<IDto>? Values { get; set; }
    public OperationStatus Status { get; set; }
    public int Count { get; set; } = 0;

    public ServiceListResult<T> With(List<IDto> values)
    {
        Values = values;
        Status = OperationStatus.Success;
        Count = values.Count;

        return this;
    }

    public ServiceListResult<T> With(OperationStatus status)
    {
        Status = status;

        return this;
    }

}
