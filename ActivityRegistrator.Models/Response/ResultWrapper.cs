namespace ActivityRegistrator.Models.Response;
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
