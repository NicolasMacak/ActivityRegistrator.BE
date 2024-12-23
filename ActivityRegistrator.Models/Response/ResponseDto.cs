namespace ActivityRegistrator.Models.Response;
public class ResponseDto<T>
{
    public T? Value { get; set; }
    public OperationStatus Status { get; set; }

    public ResponseDto<T> With(T value)
    {
        Value = value;
        Status = OperationStatus.Success;

        return this;
    }

    public ResponseDto<T> With(OperationStatus status)
    {
        Status = status;

        return this;
    }
}
