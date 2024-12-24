using ActivityRegistrator.API.Core;
using ActivityRegistrator.API.Service;
using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    private const string TenantCode = "TangoVida";

    public UsersController(ILogger<UsersController> logger, UserService userService, IMapper mapper)
    {
        _logger = logger;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        ResponseDtoList<UserEntity> response = await _userService.GetList(TenantCode);

        if(response.Status != OperationStatus.Success)
        {
            return StatusCode(500);
        }

        IEnumerable<UserDto> records = _mapper.Map<IEnumerable<UserDto>>(response.Values);
        return Ok(new { records, response.Count });
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email)
    {
        ResponseDto<UserEntity> response = await _userService.Get(TenantCode, email);

        if(response.Status == OperationStatus.NotFound)
        {
            return NotFound(ErrorBuilder.NotFoundError(new Dictionary<string, object>() {
                { "email", email }
            }));
        }

        return Ok(_mapper.Map<UserDto>(response.Value));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto requestDto)
    {
        if (requestDto == null)
        {
            return BadRequest("Person data cannot be null");
        }

        ResponseDto<UserEntity> response = await _userService.Create(TenantCode, requestDto);

        return response.Status switch
        {
            OperationStatus.Success => CreatedAtAction(nameof(Create), response),
            OperationStatus.AlreadyExists => BadRequest(ErrorBuilder.AlreadyExistsError(new Dictionary<string, object>() {
                                            { "Email", requestDto.Email }
            })),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{email}")]
    public async Task<IActionResult> Update(string email, [FromBody] UpdateUserRequestDto requestDto)
    {
        if (requestDto == null)
        {
            return BadRequest("User data cannot be null");
        }

        ResponseDto<UserEntity> response = await _userService.Update(TenantCode, email, requestDto);

        return response.Status switch
        {
            OperationStatus.Success => Ok(response.Value),
            OperationStatus.AlreadyUpdated => BadRequest(ErrorBuilder.AlreadyUpdatedError(new Dictionary<string, object>() {
                                { "RequestETag", requestDto.ETag },
                                { "DatabaseEntityEtag", response.Value!.ETag }
            })),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> Delete(string email)
    {
        ResponseDto<UserEntity> response = await _userService.Delete(TenantCode, email);

        return response.Status switch
        {
            OperationStatus.Success => NoContent(),
            OperationStatus.NotFound => NotFound(ErrorBuilder.NotFoundError(new Dictionary<string, object>() {
                                { "Email", email }
            })),
            _ => StatusCode(500)
        };
    }
}
