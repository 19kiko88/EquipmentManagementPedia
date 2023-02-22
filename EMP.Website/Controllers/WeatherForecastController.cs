using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using EMP.Service;
using EMP.Service.Models;
using Newtonsoft.Json;
using EMP.Service.Interfaces;

namespace EMP.Website.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private IEqmAFortyCompareService _service;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IEqmAFortyCompareService service
            )
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}