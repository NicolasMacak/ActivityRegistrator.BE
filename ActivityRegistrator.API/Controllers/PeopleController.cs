using ActivityRegistrator.Models.DanceStudio;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ILogger<PeopleController> _logger;
    private readonly TableClient _tableClient;

    public PeopleController(ILogger<PeopleController> logger, TableServiceClient tableServiceClient)
    {
         _logger = logger;
        _tableClient = tableServiceClient.GetTableClient("People");
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        List<Person> result = _tableClient.Query<Person>().ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        List<Person> result = _tableClient.Query<Person>(x => x.PartitionKey == id).ToList();

        return Ok(result);
    }

    // POST: api/people
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Person person)
    {
        if (person == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        try
        {
            // Add new person to the table
            await _tableClient.AddEntityAsync(person);
            return CreatedAtAction(nameof(Get), new { id = person.PartitionKey }, person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new person.");
            return StatusCode(500, "Internal server error while creating the person.");
        }
    }

    // PUT: api/people/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Person person)
    {
        if (person == null)
        {
            return BadRequest("Person data cannot be null.");
        }

        if (id != person.PartitionKey)
        {
            return BadRequest("ID in URL must match the ID in the body.");
        }

        try
        {
            // Attempt to update the person in the table
            var existingPerson = _tableClient.Query<Person>(x => x.PartitionKey == id).FirstOrDefault();

            if (existingPerson == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            // Update the person entity
            await _tableClient.UpdateEntityAsync(person, person.ETag); // react if etag was not the same
            return Ok(person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the person.");
            return StatusCode(500, "Internal server error while updating the person.");
        }
    }

    // DELETE: api/people/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            Person? person = _tableClient.Query<Person>(x => x.PartitionKey == id).FirstOrDefault();

            if (person == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            // Delete the person by PartitionKey and RowKey
            await _tableClient.DeleteEntityAsync(person);

            return NoContent(); // Successfully deleted
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the person.");
            return StatusCode(500, "Internal server error while deleting the person.");
        }
    }

}
