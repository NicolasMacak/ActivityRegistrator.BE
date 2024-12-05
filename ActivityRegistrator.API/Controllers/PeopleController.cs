using ActivityRegistrator.Models.DanceStudio;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers;
public class PeopleController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly TableServiceClient _tableServiceClient;

    public PeopleController(ILogger<WeatherForecastController> logger, TableServiceClient tableServiceClient)
    {
        _logger = logger;
        _tableServiceClient = tableServiceClient;
    }

    [HttpGet]
    public async Task<IEnumerable<Person>> GetPersons()
    {

        return new List<Person>();
    }
}
