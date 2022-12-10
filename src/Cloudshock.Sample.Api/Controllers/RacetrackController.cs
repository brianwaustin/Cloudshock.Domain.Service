using Microsoft.AspNetCore.Mvc;

namespace Cloudshock.Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RacetrackController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<RacetrackController> _logger;
    
    public RacetrackController(ILogger<RacetrackController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "GetRacetrack")]
    public IEnumerable<Racetrack> Get()
    {
        
        var Racetrack = Enumerable.Range(1, 5).Select(index => new Racetrack
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
        
        _logger.LogInformation("{@Racetrack}", Racetrack);
        
        return Racetrack;
    }

    [HttpPost(Name = "CreateRacetrack")]
    public IEnumerable<Racetrack> Post(Racetrack Racetrack)
    {
        var updatedRacetrack = new Racetrack[0];

        try{

            if(String.IsNullOrEmpty(Racetrack.Summary)) throw new Exception("Invalid Request");

            updatedRacetrack = Enumerable.Range(1, 5).Select(index => new Racetrack
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();           

        }
        catch(Exception exception)
        {           
           _logger.LogError("{@exception}",exception);
        }

        return updatedRacetrack;        
    }

}
