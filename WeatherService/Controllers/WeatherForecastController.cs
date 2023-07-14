using Common.ErrorHandling;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            // System error with 500
            return new WeatherService().FakeForcastServiceCall();
        }

        [HttpGet]
        public void Forecast3()
        {
            // User input/validation error with 400
            new WeatherService().MakePhoneCall("123");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Forecast4()
        {
            // Any other unexpected Exception captured and converted at middleware
            throw new Exception("XXXXXXXXXXXXXXXXxxxxx");
        }

        // Fake service
        public class WeatherService
        {
            // System error
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

            // User error
            public void MakePhoneCall(string x)
            {
                if (x.Length != 9)
                {
                    // Pack for frontend, if needed
                    var forFrontend = new
                    {
                        FieldName = "Phone number",
                        Message = "Must be length 9"
                    };

                    var ex = new CodedException("WeatherService.PhoneFormatWrong", "", new { DemoValidationError = forFrontend });
                    ex.HttpErrorCode = CodedException.HttpErrorCodes.UserError;  // Indicate the right http code 
                    throw ex;
                }
            }
        }
    }
}