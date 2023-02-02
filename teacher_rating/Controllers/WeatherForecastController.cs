using Microsoft.AspNetCore.Mvc;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ITeacherRepository _teacherRepository;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITeacherRepository teacherRepository)
    {
        _logger = logger;
        _teacherRepository = teacherRepository;
    }

    [HttpGet("xxqxqxq")]
    public async Task<IActionResult>  GetChangeVehicleByRegistrationTransport()
    {
        return Ok(await _teacherRepository.GetAllTeachers());
    }
    
    
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}