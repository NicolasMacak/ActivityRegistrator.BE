using ActivityRegistrator.API.Core.DataProcessing.Constants;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace ActivityRegistrator.Test.Service;
public class EnvironmentServiceTests
{
    EnvironmentService _testee;
    Mock<IEnvironmentRepository> _repositoryMock;

    public EnvironmentServiceTests() {
        _repositoryMock = new Mock<IEnvironmentRepository>();
        _testee = new(new Mock<ILogger<EnvironmentService>>().Object, _repositoryMock.Object);
    }

    [Fact]
    public void CreateTables_WhenCalled_AllTablesAreCreated()
    {
        // Arrange
        List<string> tables = new()
        {
            TablesNames.Users,
            TablesNames.Events
        };

        // Act
        _testee.SetupEnvironment();

        // Assert
        _repositoryMock.Verify(x => x.CreateInitialTablesIfNotExist(tables));
    }
}
