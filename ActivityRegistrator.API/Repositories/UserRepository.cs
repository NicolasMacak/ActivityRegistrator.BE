using ActivityRegistrator.Models.Entities;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Repositories;
public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    private const string TableName = "Users";

    public UserRepository(ILogger<GenericRepository<UserEntity>> logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient, TableName)
    {
    }
}
