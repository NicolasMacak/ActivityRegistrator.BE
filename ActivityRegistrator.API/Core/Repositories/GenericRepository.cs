using ActivityRegistrator.API.Core.Response;
using ActivityRegistrator.Models.DanceStudio;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Core.Repositories;
public class GenericRepository<Entity> where Entity : class, ITableEntity
{
    private readonly ILogger<GenericRepository<Entity>> _logger;
    private readonly TableServiceClient _tableServiceClient;

    public GenericRepository(ILogger<GenericRepository<Entity>> logger, TableServiceClient tableServiceClient)
    {
        _logger = logger;
        _tableServiceClient = tableServiceClient;
    }

    public async Task<ResponseDtoList<Entity>> GetList(string tenantCode)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(tenantCode);
        List<Entity> result = tableClient
            .Query<Entity>()
            .ToList();

        return new ResponseDtoList<Entity>().With(result);
    }

    public async Task<ResponseDto<Entity>> GetByPartitionKey(string tableName, string partitionKey)
    {
        ResponseDto<Entity> response = new();

        TableClient tableClient = _tableServiceClient.GetTableClient(tableName);
        Entity? result = tableClient.Query<Entity>(x => x.PartitionKey == partitionKey).FirstOrDefault();

        if(result == null)
        {
            return response.With(OperationStatus.NotFound);
        }

        return response.With(result);
    }

    public async Task<ResponseDto<Entity>> Create(string tableName, Entity entity)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(tableName);
        ResponseDto<Entity> response = new();

        try
        {
            await tableClient.AddEntityAsync(entity);
            return response.With(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new person.");
            return response.With(OperationStatus.Failure);
        }
    }

    public async Task<ResponseDto<Entity>> Update(string tableName, string partitionKey, Entity entity)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(tableName);
        ResponseDto<Entity> response = new();

        try
        {
            // Attempt to update the person in the table
            var existingPerson = tableClient.Query<Entity>(x => x.PartitionKey == partitionKey).FirstOrDefault();

            if (existingPerson == null)
            {
                return response.With(OperationStatus.NotFound);
            }

            // Update the person entity
            await tableClient.UpdateEntityAsync(entity, entity.ETag); // react if etag was not the same
            return response.With(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the person.");
            return response.With(OperationStatus.Failure);
        }
    }

    public async Task<ResponseDto<Entity>> Delete(string tableName, string id)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(tableName);
        ResponseDto<Entity> response = new();

        try
        {
            User? person = tableClient.Query<User>(x => x.PartitionKey == id).FirstOrDefault();

            if (person == null)
            {
                return response.With(OperationStatus.NotFound);
            }

            await tableClient.DeleteEntityAsync(person);

            return response.With(OperationStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the person.");
            return response.With(OperationStatus.Failure);
        }
    }
}
