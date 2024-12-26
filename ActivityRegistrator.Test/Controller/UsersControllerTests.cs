using System.Runtime.CompilerServices;
using ActivityRegistrator.API.Controllers;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Service;
using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.MappingProfiles;
using ActivityRegistrator.Models.ObjectResults;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.Models.Response;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ActivityRegistrator.Test.Controller;
public class UsersControllerTests
{
    private readonly UsersController _testee;
    private readonly Mock<IUserRepository> _repositoryMock = new();

    private const string Email = "caspian.holstrom@gmail.com";


    public UsersControllerTests()
    {
        MapperConfiguration configuration = new MapperConfiguration(cfg => {
            cfg.AddProfile<UserProfile>();
        });
        IMapper autoMapper = configuration.CreateMapper();

        UserService userService = new(new Mock<ILogger<UserService>>().Object, _repositoryMock.Object);
        _testee = new UsersController(new Mock<ILogger<UsersController>>().Object, userService, autoMapper);
    }

    [Fact]
    public async Task GetListAsync_WhenSucess_ReturnsOK()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetListAsync(It.IsAny<string>()))
            .ReturnsAsync(new ResultListWrapper<UserEntity>().With(new List<UserEntity>() { GetTestUserEntity() }));

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
        Assert.NotNull(response);

        Assert.Equal(1, response.Count);

        UserDto record = response.Records.First();
        Assert.Equal("TenantName", record.TenantName);
        Assert.Equal("FullName", record.FullName);
        Assert.Equal("Email", record.Email);
        Assert.Equal(Azure.ETag.All, record.ETag);
    }

    [Fact]
    public async Task GetListAsync_WhenSystemFailure_Returns500()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetListAsync(It.IsAny<string>()))
            .ReturnsAsync(new ResultListWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        StatusCodeResult? response = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(response);

        Assert.Equal(500, response.StatusCode);
    }

    [Fact]
    public async Task GetAsync_WhenSucess_ReturnsOK()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity()));

        // Act
        IActionResult result = await _testee.GetAsync(Email);

        // Assert
        UserDto? response = Assert.IsType<OkObjectResult>(result).Value as UserDto;
        Assert.NotNull(response);

        Assert.Equal("TenantName", response.TenantName);
        Assert.Equal("FullName", response.FullName);
        Assert.Equal("Email", response.Email);
        Assert.Equal(ETag.All, response.ETag);
    }

    [Fact]
    public async Task GetAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.NotFound));

        // Act
        IActionResult result = await _testee.GetAsync(Email);

        // Assert
        Dictionary<string, object>? error = Assert.IsType<NotFoundObjectResult>(result).Value as Dictionary<string, object>;

        Assert.NotNull(error);
        Assert.Equal("Resource not found", error["message"]);
    }

    [Fact]
    public async Task GetAsync_WhenSystemError_Returns500()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.GetAsync(Email);

        // Assert
        StatusCodeResult? response = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(response);

        Assert.Equal(500, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_WhenSuccess_ReturnsCreated()
    {
        // Arrange
        CreateUserRequestDto request = new()
        {
            Email = Email,
            FullName = "FullName"
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.NotFound));

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity()));

        // Act
        IActionResult result = await _testee.CreateAsync(request);

        // Assert
        UserDto? response = Assert.IsType<CreatedAtActionResult>(result).Value as UserDto;
        Assert.NotNull(response);

        Assert.Equal("TenantName", response.TenantName);
        Assert.Equal("FullName", response.FullName);
        Assert.Equal("Email", response.Email);
        Assert.Equal(ETag.All, response.ETag);
    }

    [Fact]
    public async Task CreateAsync_WhenTenantAlreadyContainsUserWithSuchEmail_ReturnsConflict()
    {
        // Arrange
        CreateUserRequestDto request = new()
        {
            Email = Email,
            FullName = "FullName"
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Success));

        // Act
        IActionResult result = await _testee.CreateAsync(request);

        // Assert
        Dictionary<string, object>? error = Assert.IsType<ConflictObjectResult>(result).Value as Dictionary<string, object>;

        Assert.NotNull(error);
        Assert.Equal("Such resource already exists", error["message"]);

        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenSystemErrorOccursDuringGetAsync_Returns500()
    {
        // Arrange
        CreateUserRequestDto request = new()
        {
            Email = Email,
            FullName = "FullName"
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.CreateAsync(request);

        // Assert
        StatusCodeResult? response = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(response);

        Assert.Equal(500, response.StatusCode);

        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenSystemErrorOccursDuringCreateAsync_Returns500()
    {
        // Arrange
        CreateUserRequestDto request = new()
        {
            Email = Email,
            FullName = "FullName"
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.CreateAsync(request);

        // Assert
        StatusCodeResult? error = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(error);

        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    public async Task PutAsync_WhenSuccess_ReturnsOK()
    {
        // Arrange
        ETag entityToUpdateETag = new ETag("ETag");

        UpdateUserRequestDto request = new()
        {
            FullName = "FullName",
            ETag = entityToUpdateETag
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity()));

        _repositoryMock.Setup(x => x.Update(Email, entityToUpdateETag, It.IsAny<UserEntity>()))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity()));

        // Act
        IActionResult result = await _testee.UpdateAsync(Email, request);

        // Assert
        UserDto? response = Assert.IsType<OkObjectResult>(result).Value as UserDto;
        Assert.NotNull(response);

        Assert.Equal("TenantName", response.TenantName);
        Assert.Equal("FullName", response.FullName);
        Assert.Equal("Email", response.Email);
        Assert.Equal(ETag.All, response.ETag);
    }

    [Fact]
    public async Task PutAsync_WhenUserWasUpdatedInTheMeantime_ReturnsPreconditionFailed()
    {
        // Arrange
        UpdateUserRequestDto request = new()
        {
            FullName = "FullName",
            ETag = new ETag("req")
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity()));

        _repositoryMock.Setup(x => x.Update(Email, It.IsAny<ETag>(), It.IsAny<UserEntity>()))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(GetTestUserEntity(), OperationStatus.UniqueConstraintViolation));

        // Act
        IActionResult result = await _testee.UpdateAsync(Email, request);

        // Assert
        Dictionary<string, object>? error = Assert.IsType<PreconditionFailedObjectResult>(result).Value as Dictionary<string, object>;
        Assert.NotNull(error);

        Assert.Equal("Concurency error. Related resource was already updated.", error["message"]);
    }

    [Fact]
    public async Task PutAsync_WhenSystemError_Returns500()
    {
        // Arrange
        UpdateUserRequestDto request = new()
        {
            FullName = "FullName",
            ETag = new ETag("ETag")
        };

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.UpdateAsync(Email, request);

        // Assert
        StatusCodeResult? error = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(error);

        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_WhenSuccess_ReturnsNoContent()
    {
        // Arrange
        var entityToDelete = GetTestUserEntity();

        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(entityToDelete));

        _repositoryMock.Setup(x => x.DeleteAsync(entityToDelete))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Success));

        // Act
        IActionResult result = await _testee.Delete(Email);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.NotFound));

        // Act
        IActionResult result = await _testee.Delete(Email);

        // Assert
        Dictionary<string, object>? error = Assert.IsType<NotFoundObjectResult>(result).Value as Dictionary<string, object>;

        Assert.NotNull(error);
        Assert.Equal("Resource not found", error["message"]);
    }

    [Fact]
    public async Task DeleteAsync_WhenSystemError_Returns500()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), Email))
            .ReturnsAsync(new ResultWrapper<UserEntity>().With(OperationStatus.Failure));

        // Act
        IActionResult result = await _testee.Delete(Email);

        // Assert
        StatusCodeResult? error = Assert.IsType<StatusCodeResult>(result);
        Assert.NotNull(error);

        Assert.Equal(500, error.StatusCode);
    }


    private UserEntity GetTestUserEntity()
    {
        return new UserEntity()
        {
            PartitionKey = "TenantName",
            RowKey = "Email",
            FullName = "FullName",
            Timestamp = new DateTime(2000, 12, 12),
            ETag = Azure.ETag.All
        };
    }
}
