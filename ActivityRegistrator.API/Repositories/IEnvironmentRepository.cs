namespace ActivityRegistrator.API.Repositories;
public interface IEnvironmentRepository
{
    /// <summary>
    /// Creates tables in Azure Table Storage, if they do not exist already
    /// </summary>
    /// <param name="tablesNames">Tables that will be created</param>
    public void CreateInitialTablesIfNotExist(IEnumerable<string> tablesNames);
}
