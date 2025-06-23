namespace ActivityRegistrator.API.Core.DataProcessing.Model;
public class ResponseListDto<T>
{
    public ResponseListDto(IEnumerable<IDto> records, int count)
    {
        Records = records;
        Count = count;
    }

    public IEnumerable<IDto> Records { get; set; }
    public int Count { get; set; }
}
