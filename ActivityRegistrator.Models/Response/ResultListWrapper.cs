namespace ActivityRegistrator.Models.Response;
/// <summary>
/// May contain Values of type <see cref="T"/> and <see cref="OperationStatus"/> providing information about operation execution
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResultListWrapper<T>
{
    public List<T>? Values { get; set; }
    public OperationStatus Status { get; set; }
    public int Count { get; set; } = 0;

    public ResultListWrapper<T> With(List<T> values)
    {
        Values = values;
        Status = OperationStatus.Success;
        Count = values.Count;

        return this;
    }

    public ResultListWrapper<T> With(OperationStatus status)
    {
        Status = Status;

        return this;
    }

}
