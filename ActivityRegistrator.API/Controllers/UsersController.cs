using ActivityRegistrator.API.Core;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly UserRepository _userRepository;

    private const string TenantCode = "TangoVida";

    public UsersController(ILogger<UsersController> logger, UserRepository userRepository)
    {
         _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        ResponseDtoList<UserEntity> response = await _userRepository.GetList(TenantCode);

        return Ok(new { response.Values, response.Count });
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email)
    {
        ResponseDto<UserEntity> response = await _userRepository.Get(TenantCode, email);

        if(response.Status == OperationStatus.NotFound)
        {
            _logger.LogError("User not found. tenantCode: {TenantCode}, email: {email}", TenantCode, email);
            return NotFound(ErrorBuilder.NotFoundError(new Dictionary<string, string>() { { "email", email } }));
        }

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserEntity user)
    {
        if (user == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        ResponseDto<UserEntity> response = await _userRepository.Create(user);

        if(response.Status == OperationStatus.Failure)
        {
            return StatusCode(500);
        }

        return Ok(response.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserEntity user)
    {
        if (user == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        if (id != user.PartitionKey)
        {
            return BadRequest("ID in URL must match the ID in the body.");
        }

        ResponseDto<UserEntity> response = await _userRepository.Update(id, user);

        if (response.Status == OperationStatus.Failure) 
        {
            return BadRequest();
        }

        return Ok(response.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        ResponseDto<UserEntity> response = await _userRepository.Delete(id);

        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(id); // bachnut dictionary
        }

        return NoContent();
    }
}
