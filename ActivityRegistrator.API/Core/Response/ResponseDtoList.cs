namespace ActivityRegistrator.API.Core.Response;
public class ResponseDtoList<T>
{
    public List<T>? Values { get; set; }
    public OperationStatus Status { get; set; }
    public int Count { get; set; } = 0;

    /// <summary>
    /// Successful operation
    /// </summary>
    public ResponseDtoList<T> With(List<T> values)
    {
        Values = values;
        Status = OperationStatus.Success;
        Count = values.Count;

        return this;
    }

    /// <summary>
    /// Unsucessful operation
    /// </summary>
    public ResponseDtoList<T> With(OperationStatus status)
    {
        Status = Status;

        return this;
    }

}
