namespace ActivityRegistrator.Models.Response;
/// <summary>
/// May contain Value of type <see cref="T"/> and <see cref="OperationStatus"/> providing information about operation execution
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResultWrapper<T>
{
    public T? Value { get; set; }
    public OperationStatus Status { get; set; }

    public ResultWrapper<T> With(T value, OperationStatus status)
    {
        Value = value;
        Status = status;

        return this;
    }

    public ResultWrapper<T> With(T value)
    {
        Value = value;
        Status = OperationStatus.Success;

        return this;
    }

    public ResultWrapper<T> With(OperationStatus status)
    {
        Status = status;

        return this;
    }
}
