namespace ActivityRegistrator.Models.Response;
public class ResultListWrapper<T>
{
    public List<T>? Values { get; set; }
    public OperationStatus Status { get; set; }
    public int Count { get; set; } = 0;

    /// <summary>
    /// Successful operation
    /// </summary>
    public ResultListWrapper<T> With(List<T> values)
    {
        Values = values;
        Status = OperationStatus.Success;
        Count = values.Count;

        return this;
    }

    /// <summary>
    /// Unsucessful operation
    /// </summary>
    public ResultListWrapper<T> With(OperationStatus status)
    {
        Status = Status;

        return this;
    }

}
