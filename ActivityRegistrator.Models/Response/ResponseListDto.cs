namespace ActivityRegistrator.Models.Response;
public class ResponseListDto<T>
{
    public ResponseListDto(IEnumerable<T> records, int count)
    {
        Records = records;
        Count = count;
    }

    public IEnumerable<T> Records { get; set; }
    public int Count { get; set; }
}
