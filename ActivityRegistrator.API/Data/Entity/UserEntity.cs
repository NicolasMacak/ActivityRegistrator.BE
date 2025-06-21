using System.Runtime.Serialization;
using ActivityRegistrator.API.Core.UserAccess.Enums;
using Azure;
using Azure.Data.Tables;

namespace ActivityRegistrator.Models.Entities;
public class UserEntity : ITableEntity
{
    [IgnoreDataMember]
    public string TenantCode {
        get => PartitionKey;
        set => PartitionKey = value;
    } 

    [IgnoreDataMember]
    public string Email {
        get => RowKey;
        set => RowKey = value;
    }

    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public string AccessLevel { get; set; } = UserAccessLevel.Guest.ToString();
    public string FullName { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
