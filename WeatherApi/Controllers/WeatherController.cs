using Microsoft.AspNetCore.Mvc;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherController : ControllerBase
  {
    private readonly IWeatherService weatherService;

    public WeatherController(IWeatherService weatherService)
    {
      this.weatherService = weatherService;
    }
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
      [FromQuery] Unit unit,
      [FromQuery] string locations,
      [FromQuery] float temperature)
    {
      var locationIds = locations.Split(',').Distinct();

      if (locationIds.Any(x => !int.TryParse(x, out _)))
      {
        return BadRequest("'locations' parameter only supports lists of integer id's separated by a comma");
      }

      try
      {
        var result = await weatherService.GetLocationsWithTemperatureAboveAsync(locationIds, unit, temperature);
        return Ok(result);
      }
      catch (LocationNotFoundException e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("locations/{id}")]
    public async Task<IActionResult> GetForecast(string id)
    {
      if (!int.TryParse(id, out _))
      {
        return BadRequest("'id' parameter must be an integer id");
      }

      try
      {
        var result = await weatherService.GetFiveDayForecast(id);
        return Ok(result);
      }
      catch (LocationNotFoundException e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}