using ActivityRegistrator.API.Controllers;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Service;
using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.MappingProfiles;
using ActivityRegistrator.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ActivityRegistrator.Test.Controller;
public class UsersControllerTests
{
    private readonly UsersController _testee;
    private readonly Mock<IUserRepository> _repositoryMock = new();

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

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetAsync_WhenSucess_ReturnsOK()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task GetAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task GetAsync_WhenCalled_Returns500()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ReturnsCreated()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task CreateAsync_WhenTenantAlreadyContainsUserWithSuchEmail_ReturnsBadRequest() // todo. Bad request?
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task CreateAsync_WhenSystemErrorOccuers_Returns500() // todo. Bad request?
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task PutAsync_WhenCalled_ReturnsOK()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task PutAsync_WhenUserWasUpdatedInTheMeantime_ReturnsBadRequest() // todo.Check error for this
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task PutAsync_WhenSystemError_Returns500()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task DeleteAsync_WhenHappyPath_ReturnsNoContent()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task PutAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
    }

    [Fact]
    public async Task DeleteAsync_WhenSystemError_Returns500()
    {
        // Arrange

        // Act
        IActionResult result = await _testee.GetListAsync();

        // Assert
        ResponseListDto<UserDto>? response = Assert.IsType<OkObjectResult>(result).Value as ResponseListDto<UserDto>;
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
