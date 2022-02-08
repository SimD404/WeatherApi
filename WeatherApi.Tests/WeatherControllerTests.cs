using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Controllers;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.Tests
{
  public class Tests
  {
    private WeatherController controller;
    private IWeatherService weatherService;

    [SetUp]
    public void Setup()
    {
      weatherService = Substitute.For<IWeatherService>();
      controller = new WeatherController(weatherService);
    }

    [Test]
    public async Task GetSummary_WhenInputIsValid_ForwardsParametersToService()
    {
      await controller.GetSummary(Unit.Celsius, "1,2,3,4,5", 0.0f);

      await weatherService.Received(1).GetLocationsWithTemperatureAboveAsync(
        Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(new[] { "1", "2", "3", "4", "5" })),
        Unit.Celsius,
        0.0f);
    }

    [TestCase("lala,2,3,4")]
    [TestCase("0.123,2,3,4")]
    [TestCase("This is broken")]
    public async Task GetSummary_WhenLocationsAreInvalid_ReturnsBadRequest(string invalidLocations)
    {
      var result = await controller.GetSummary(Unit.Celsius, invalidLocations, 0.0f);

      Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetSummary_WhenInputContainsDuplicateIds_FiltersOutDuplicateIds()
    {
      await controller.GetSummary(Unit.Celsius, "1,1,2,2,3,4,5", 0.0f);

      await weatherService.Received(1).GetLocationsWithTemperatureAboveAsync(
        Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(new[] { "1", "2", "3", "4", "5"})),
        Unit.Celsius,
        0.0f);
    }

    [Test]
    public async Task GetSummary_WhenServiceThrowsLocationNotFound_ReturnsBadRequest()
    {
      weatherService
        .WhenForAnyArgs(x => x.GetLocationsWithTemperatureAboveAsync(default, default, default))
        .Do(x => { throw new LocationNotFoundException("123"); });
      
      var result = await controller.GetSummary(Unit.Celsius, "1", 0.0f);
      Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetForecast_WhenServiceThrowsLocationNotFound_ReturnsBadRequest()
    {
      weatherService
        .WhenForAnyArgs(x => x.GetFiveDayForecast(default))
        .Do(x => { throw new LocationNotFoundException("123"); });

      var result = await controller.GetForecast("1");
      Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [TestCase("1,2,3,4")]
    [TestCase("0.123")]
    [TestCase("This is broken")]
    public async Task GetForecast_WhenLocationIsInvalid_ReturnsBadRequest(string invalidLocation)
    {
      var result = await controller.GetForecast(invalidLocation);

      Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetForecast_WhenInputIsValid_ForwardsParametersToService()
    {
      await controller.GetForecast("1");
      await weatherService.Received(1).GetFiveDayForecast("1");
    }
  }
}