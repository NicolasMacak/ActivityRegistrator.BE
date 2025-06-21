using Azure;
using Azure.Data.Tables;

namespace ActivityRegistrator.Models.Entities;
public class UserEntity : ITableEntity
{   
    public string TenantCode {
        get => PartitionKey;
        set => PartitionKey = value;
    } 

    public string Email {
        get => RowKey;
        set => RowKey = value;
    }

    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public string AccessRole { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
