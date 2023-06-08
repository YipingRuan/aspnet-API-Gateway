using Common.ErrorHandling;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;

namespace WeatherService.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Forecast()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Forecast2()
        {
            return new WeatherService().FakeForcastServiceCall();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Forecast3()
        {
            _logger.LogError("Dead");
            LogUtility.LoggerFactory.CreateLogger<WeatherForecast>().LogInformation("Dead2");
            throw new Exception("XXXXXXXXXXXXXXXXxxxxx");
        }

        // Fake service
        public class WeatherService
        {
            public IEnumerable<WeatherForecast> FakeForcastServiceCall()
            {
                string fileName = "NoSuchFile.txt";
                try
                {
                    System.IO.File.OpenText(fileName);  // Somewhere deep in the code
                }
                catch (Exception ex)
                {
                    throw ex.Bag("WeatherService.ForecastFail", new { FileName = fileName });
                }

                return null;
            }
        }
    }
}