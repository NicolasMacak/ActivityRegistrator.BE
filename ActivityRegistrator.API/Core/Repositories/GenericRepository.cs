using System.Net;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using Azure;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Core.Repositories;
public class GenericRepository<Entity> where Entity : class, ITableEntity
{
    private readonly string _tableName;

    private readonly ILogger<GenericRepository<Entity>> _logger;
    private readonly TableServiceClient _tableServiceClient;

    public GenericRepository(ILogger<GenericRepository<Entity>> logger, TableServiceClient tableServiceClient, string tableName)
    {
        _logger = logger;
        _tableServiceClient = tableServiceClient;
        _tableName = tableName;
    }

    public async Task<ResponseDtoList<Entity>> GetList(string tenantCode)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(_tableName);
        List<Entity> result = tableClient
            .Query<Entity>(x => x.PartitionKey == tenantCode)
            .ToList();

        return new ResponseDtoList<Entity>().With(result);
    }

    public async Task<ResponseDto<Entity>> Get(string partitionKey, string rowKey)
    {
        ResponseDto<Entity> response = new();

        TableClient tableClient = _tableServiceClient.GetTableClient(_tableName);

        try
        {
            Entity? result = tableClient.GetEntity<Entity>(partitionKey, rowKey);
            return response.With(result);
        }
        catch (RequestFailedException requestFailedException)
        {
            if(requestFailedException.Status == (int)HttpStatusCode.NotFound)
            {
                return response.With(OperationStatus.NotFound);
            }
            throw;
        }
        catch (Exception exception)
        {
            throw;
        }
    }

    public async Task<ResponseDto<Entity>> Create(Entity entity)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(_tableName);
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

    public async Task<ResponseDto<Entity>> Update(string partitionKey, Entity entity)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(_tableName);
        ResponseDto<Entity> response = new();

        try
        {
            var existingPerson = tableClient.Query<Entity>(x => x.PartitionKey == partitionKey).FirstOrDefault();

            if (existingPerson == null)
            {
                return response.With(OperationStatus.NotFound);
            }

            await tableClient.UpdateEntityAsync(entity, entity.ETag); //todo. react if etag was not the same
            return response.With(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the person.");
            return response.With(OperationStatus.Failure);
        }
    }

    public async Task<ResponseDto<Entity>> Delete(string id)
    {
        TableClient tableClient = _tableServiceClient.GetTableClient(_tableName);
        ResponseDto<Entity> response = new();

        try
        {
            UserEntity? person = tableClient.Query<UserEntity>(x => x.PartitionKey == id).FirstOrDefault();

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
