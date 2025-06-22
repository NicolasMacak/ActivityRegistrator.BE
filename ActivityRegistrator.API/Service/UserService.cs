using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Core.Security.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;
using Optional;

namespace ActivityRegistrator.API.Service;
public class UserService : IUserService // TenantAdmin service
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IActiveUserService _activeUserService;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IActiveUserService activeUserService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _activeUserService = activeUserService;
    }

    /// <inheritdoc/>
    public async Task<ResultListWrapper<UserEntity>> GetListAsync()
    {
        try
        {
            List<UserEntity> userEntities = await _userRepository.GetListAsync(_activeUserService.TenantCode);

            return new ResultListWrapper<UserEntity>
            {
                Values = userEntities,
                Count = userEntities.Count,
                Status = OperationStatus.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user list for tenant: {TenantCode}", _activeUserService.TenantCode);
            return new ResultListWrapper<UserEntity>().With(OperationStatus.Failure);
        }

    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> GetAsync(string email)
    {
        try
        {
            Option<UserEntity> optionUser = await _userRepository.GetAsync(_activeUserService.TenantCode, email);

            return optionUser.Match(
                some: user => new ResultWrapper<UserEntity>().With(user),
                none: () => new ResultWrapper<UserEntity>().With(OperationStatus.NotFound)
            );
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving user with email: {Email} for tenant: {TenantCode}", email, _activeUserService.TenantCode);
            return new ResultWrapper<UserEntity>().With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> CreateAsync(CreateUserRequestDto requestDto)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.GetAsync(_activeUserService.TenantCode, requestDto.Email);

        if (responseOfEntityToUpdate.Status == OperationStatus.Success)
        {
            _logger.LogError("Such resource already exists. request: {requestDto}", requestDto.ToString());
            return new ResultWrapper<UserEntity>().With(OperationStatus.UniqueConstraintViolation);
        }
        else if (responseOfEntityToUpdate.Status == OperationStatus.Failure)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity newUserEntity = new()
        {
            PartitionKey = tenantCode,
            RowKey = requestDto.Email,
            FullName = requestDto.FullName,
            AccessLevel = UserAccessLevel.User.ToString()
        };

        return await _userRepository.CreateAsync(newUserEntity);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> UpdateAsync(string email, UpdateUserRequestDto request)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.GetAsync(tenantCode, email);

        if(responseOfEntityToUpdate.Status != OperationStatus.Success)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity entityToUpdate = responseOfEntityToUpdate.Value!;

        entityToUpdate.FullName = request.FullName;

        return await _userRepository.Update(email, request.ETag, entityToUpdate);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> DeleteAsync(string email)
    {
        ResultWrapper<UserEntity> entityToDeleteResponse = await _userRepository.GetAsync(tenantCode, email);
        if (entityToDeleteResponse.Status != OperationStatus.Success)
        {
            return entityToDeleteResponse;
        }

        return await _userRepository.DeleteAsync(entityToDeleteResponse.Value!);
    }
}
