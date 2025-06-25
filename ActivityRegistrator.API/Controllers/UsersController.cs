using System.Net;
using ActivityRegistrator.API.Service;
using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ActivityRegistrator.API.Core.DataProcessing.Builders.ObjectResultBuilder;
using ActivityRegistrator.API.Core.Security.Constants;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;
using ActivityRegistrator.API.Core.DataProcessing.Builders;

namespace ActivityRegistrator.API.Controllers;

[Authorize(Policy = UserAccessLevelPolicy.TenantAdminAccess)]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        ServiceListResult<UserEntity> response = await _userService.GetListAsync();

        if(response.Status != OperationStatus.Success)
        {
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        return Ok(new ResponseListDto<UserDto>( // can be dynamic?
            records: response.Values!,
            count: response.Count));
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetAsync(string email)
    {
        ServiceResult<UserEntity> response = await _userService.GetAsync(email);

        return response.Status switch
        {
            OperationStatus.Success => Ok(response.Value),
            OperationStatus.NotFound => NotFound(ErrorBuilder.NotFoundError(new Dictionary<string, object>() {
                { "email", email }
            })),
            _ => StatusCode((int) HttpStatusCode.InternalServerError)
        };
    }

    [HttpPost]
    [ActionName(nameof(CreateAsync))]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequestDto requestDto)
    {
        if (requestDto == null) // todo. later returned by fluent validation
        {
            return BadRequest("Person data cannot be null");
        }

        ServiceResult<UserEntity> response = await _userService.CreateAsync(requestDto);

        return response.Status switch
        {
            OperationStatus.Success => CreatedAtAction(nameof(CreateAsync), response.Value),
            OperationStatus.UniqueConstraintViolation => Conflict(
                ErrorBuilder.AlreadyExistsError(new Dictionary<string, object>() { { "Email", requestDto.Email }
            })),
            _ => StatusCode((int) HttpStatusCode.InternalServerError)
        };
    }


    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateAsync(string email, [FromBody] UpdateUserRequestDto requestDto)
    {
        if (requestDto == null)
        {
            return BadRequest("User data cannot be null");
        }

        ServiceResult<UserEntity> response = await _userService.UpdateAsync(email, requestDto);

        return response.Status switch
        {
            OperationStatus.Success => Ok(response.Value),
            OperationStatus.UniqueConstraintViolation => PreconditionFailed(ErrorBuilder.AlreadyUpdatedError(new Dictionary<string, object>() {
                                { "RequestETag", requestDto.ETag }
            })),
            _ => StatusCode((int) HttpStatusCode.InternalServerError)
        };
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> Delete(string email)
    {
        ServiceResult<UserEntity> response = await _userService.DeleteAsync(email);

        return response.Status switch
        {
            OperationStatus.Success => NoContent(),
            OperationStatus.NotFound => NotFound(ErrorBuilder.NotFoundError(new Dictionary<string, object>() {
                                { "Email", email }
            })),
            _ => StatusCode((int) HttpStatusCode.InternalServerError)
        };
    }
}
