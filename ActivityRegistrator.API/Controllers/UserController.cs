using ActivityRegistrator.API.Core.Response;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.DanceStudio;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserRepository _userRepository;

    private const string TableName = "People";

    public UserController(ILogger<UserController> logger, UserRepository userRepository)
    {
         _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        ResponseDtoList<User> response = await _userRepository.GetList(TableName);

        return Ok(new { response.Values, response.Count });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        ResponseDto<User> response = await _userRepository.GetByPartitionKey(TableName, id);

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User person)
    {
        if (person == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        ResponseDto<User> response = await _userRepository.Create(TableName, person);

        if(response.Status == OperationStatus.Failure) // todo. Duplicity constraint
        {
            return BadRequest(); // hodit 500
        }

        return Ok(response.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] User person)
    {
        if (person == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        if (id != person.PartitionKey)
        {
            return BadRequest("ID in URL must match the ID in the body.");
        }

        ResponseDto<User> response = await _userRepository.Update(TableName, id, person);

        if (response.Status == OperationStatus.Failure) 
        {
            return BadRequest();
        }

        return Ok(response.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        ResponseDto<User> response = await _userRepository.Delete(TableName, id);

        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(id); // bachnut dictionary
        }

        return NoContent();
    }

}
