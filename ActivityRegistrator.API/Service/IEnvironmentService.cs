using ActivityRegistrator.API.Repositories;

namespace ActivityRegistrator.API.Service;
public interface IEnvironmentService
{
    /// <inheritdoc cref="IEnvironmentRepository.CreateInitialTablesIfNotExist(IEnumerable{string})" />
    public void SetupEnvironment();
}
