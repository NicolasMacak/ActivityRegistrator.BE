namespace ActivityRegistrator.API.Core.DataProcessing.Model;
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
