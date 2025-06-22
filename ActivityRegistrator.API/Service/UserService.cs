using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Core.Security.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;
using Optional;
using Optional.Unsafe;
using Azure;
using System.Net;

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
    public async Task<ServiceListResult<UserEntity>> GetListAsync()
    {
        try
        {
            List<UserEntity> userEntities = await _userRepository.GetListAsync(_activeUserService.TenantCode);

            return new ServiceListResult<UserEntity>
            {
                Values = userEntities,
                Count = userEntities.Count,
                Status = OperationStatus.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user list for tenant: {TenantCode}", _activeUserService.TenantCode);
            return new ServiceListResult<UserEntity>().With(OperationStatus.Failure);
        }

    }

    /// <inheritdoc/>
    public async Task<ServiceResult<UserEntity>> GetAsync(string email)
    {
        try
        {
            Option<UserEntity> optionUser = await _userRepository.GetAsync(_activeUserService.TenantCode, email);

            return optionUser.Match(
                some: user => new ServiceResult<UserEntity>().With(user),
                none: () => new ServiceResult<UserEntity>().With(OperationStatus.NotFound)
            );
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving user with email: {Email} for tenant: {TenantCode}", email, _activeUserService.TenantCode);
            return new ServiceResult<UserEntity>().With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<UserEntity>> CreateAsync(CreateUserRequestDto requestDto)
    {
        try
        {
            Option<UserEntity> existingUser = await _userRepository.GetAsync(_activeUserService.TenantCode, requestDto.Email);

            if (existingUser.HasValue)
            {
                _logger.LogError("Such resource already exists. request: {requestDto}", requestDto.ToString());
                return new ServiceResult<UserEntity>().With(OperationStatus.UniqueConstraintViolation);
            }

            UserEntity newUserEntity = new()
            {
                PartitionKey = _activeUserService.TenantCode,
                RowKey = requestDto.Email,
                FullName = requestDto.FullName,
                AccessLevel = UserAccessLevel.User.ToString()
            };

            await _userRepository.CreateAsync(newUserEntity);

            Option<UserEntity> createdUser = await _userRepository.GetAsync(_activeUserService.TenantCode, requestDto.Email);

            return createdUser.Match(
                some: user => new ServiceResult<UserEntity>().With(user),
                none: () => new ServiceResult<UserEntity>().With(OperationStatus.Failure)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking for existing user with email: {Email} for tenant: {TenantCode}", requestDto.Email, _activeUserService.TenantCode);
            return new ServiceResult<UserEntity>().With(OperationStatus.Failure);
        }

    }

    /// <inheritdoc/>
    public async Task<ServiceResult<UserEntity>> UpdateAsync(string email, UpdateUserRequestDto request)
    {
        try
        {
            Option<UserEntity> existingUser = await _userRepository.GetAsync(_activeUserService.TenantCode, email);

            if (!existingUser.HasValue)
            {
                _logger.LogWarning("User not found for update: {Email}", email);
                return new ServiceResult<UserEntity>().With(OperationStatus.NotFound);
            }

            UserEntity userToUpdate = existingUser.ValueOrDefault();

            userToUpdate.FullName = request.FullName;

            await _userRepository.UpdateAsync(_activeUserService.TenantCode, request.ETag, userToUpdate);

            Option<UserEntity> updatedUser = await _userRepository.GetAsync(_activeUserService.TenantCode, email);

            return new ServiceResult<UserEntity>().With(updatedUser.ValueOrFailure());
        }
        catch (RequestFailedException requestFailedException)
        {
            if (requestFailedException.Status == (int)HttpStatusCode.PreconditionFailed) // Already updated
            {
                _logger.LogError(requestFailedException, "Record was already updated. partitionKey: {partitionKey}, rowkey: {rowKey}", _activeUserService.TenantCode, email);
                return new ServiceResult<UserEntity>().With(OperationStatus.AlreadyUpdated);
            }

            _logger.LogError(requestFailedException, "An error occurred while updating the user");
            return new ServiceResult<UserEntity>().With(OperationStatus.Failure);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating user with email: {Email} for tenant: {TenantCode}", email, _activeUserService.TenantCode);
            return new ServiceResult<UserEntity>().With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<UserEntity>> DeleteAsync(string email)
    {
        try
        {
            Option<UserEntity> optionUser = await _userRepository.GetAsync(_activeUserService.TenantCode, email);

            return await optionUser.Match(
                some: async userToDelete =>
                {
                    await _userRepository.DeleteAsync(userToDelete);
                    return new ServiceResult<UserEntity>().With(OperationStatus.Success);
                },
                none: () => {
                    return Task.FromResult(new ServiceResult<UserEntity>().With(OperationStatus.Success));
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"An error occurred while updating user with email: {Email} for tenant: {TenantCode}", email, _activeUserService.TenantCode);
            return new ServiceResult<UserEntity>().With(OperationStatus.Failure);
        }
    }
}
