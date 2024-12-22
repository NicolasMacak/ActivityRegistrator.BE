using ActivityRegistrator.API.Core.Repositories;
using ActivityRegistrator.Models.DanceStudio;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Repositories;
public class UserRepository : GenericRepository<User>
{
    public UserRepository(ILogger<GenericRepository<User>> logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient)
    {
    }
}
