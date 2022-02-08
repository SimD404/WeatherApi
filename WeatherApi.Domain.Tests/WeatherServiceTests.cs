using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Common;

namespace WeatherApi.Domain.Tests
{
  public class WeatherServiceTests
  {
    private WeatherService weatherService;
    private IWeatherDataCache weatherDataCacheMock;
    private IWeatherDataClient weatherDataClientMock;
    private IDateTimeWrapper dateTimeWrapper;

    [SetUp]
    public void Setup()
    {
      weatherDataCacheMock = Substitute.For<IWeatherDataCache>();
      weatherDataClientMock = Substitute.For<IWeatherDataClient>();

      dateTimeWrapper = Substitute.For<IDateTimeWrapper>();
      dateTimeWrapper.UtcNow.Returns(DateTime.UtcNow);

      weatherService = new WeatherService(weatherDataCacheMock, weatherDataClientMock, dateTimeWrapper);
    }

    [Test]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenCacheIsValid_ClientIsNotCalled()
    {
      var theLocationId = "aLocationId";
      var theLocation = new Location(
          theLocationId, new[]
          {
            new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), 0.0f, Unit.Celsius)
          });

      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((true, theLocation));

      await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);

      await weatherDataClientMock.DidNotReceive().GetFiveDayForecastAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenCacheIsEmpty_DataIsRetrievedFromClientAndSavedToCache()
    {
      var theLocationId = "aLocationId";

      weatherDataCacheMock
        .TryGetLocationAsync(theLocationId)
        .Returns((false, null));

      var upToDateLocation = new Location(
        theLocationId,
        new[] { new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1),
        0.0f,
        Unit.Celsius) });

      weatherDataClientMock
        .GetFiveDayForecastAsync(theLocationId)
        .Returns(upToDateLocation);

      await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);

      await weatherDataClientMock.Received(1).GetFiveDayForecastAsync(theLocationId);
      await weatherDataCacheMock.Received(1).UpdateAsync(Arg.Is<Location>(
        x => string.Equals(x.Id, upToDateLocation.Id) && x.Forecasts.First().Equals(upToDateLocation.Forecasts.First())));
    }

    [Test]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenCacheIsOutdated_DataIsRetrievedFromClientAndSavedToCache()
    {
      var theLocationId = "aLocationId";
      var locationFromCache = new Location(theLocationId, new[] { new Forecast(DateTime.UtcNow, 0.0f, Unit.Celsius) });
      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((true, locationFromCache));

      var upToDateLocation = new Location(
        theLocationId,
        new[] { new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1),
        0.0f,
        Unit.Celsius) });

      weatherDataClientMock
        .GetFiveDayForecastAsync(Arg.Any<string>())
        .Returns(upToDateLocation);

      await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);
      await weatherDataCacheMock.Received(1).UpdateAsync(Arg.Is<Location>(
        x => string.Equals(x.Id, upToDateLocation.Id) && x.Forecasts.First().Equals(upToDateLocation.Forecasts.First())));
    }

    [Test]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenCacheIsValid_ForecastsAreFilteredOnProvidedMinTemperature()
    {
      var theLocationId = "aLocationId";

      var forecastBelow = new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), -1.0f, Unit.Celsius);
      var forecastOnEdge = new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), 0.0f, Unit.Celsius);
      var forecastAbove = new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), 1.0f, Unit.Celsius);

      var theLocation = new Location(theLocationId, new[] { forecastBelow, forecastOnEdge, forecastAbove });

      weatherDataCacheMock
        .TryGetLocationAsync(theLocationId)
        .Returns((true, theLocation));

      var result = await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);

      Assert.That(result.ContainsKey(theLocationId));
      Assert.That(result[theLocationId].Count == 1);
      Assert.That(result[theLocationId].Contains(forecastAbove));
    }

    [TestCase(30.0f, 86.0f)]
    [TestCase(40.0f, 104.0f)]
    [TestCase(50.0f, 122.0f)]
    [TestCase(60.0f, 140.0f)]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenCelciusIsRequested_ConvertsFromFahrenheitToCelsius(float expectedValue, float valueInCache)
    {
      var theLocationId = "aLocationId";
      var theLocation = new Location(theLocationId, new[]
      {
        new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), valueInCache, Unit.Fahrenheit)
      });

      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((true, theLocation));

      var result = await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);
      Assert.AreEqual(expectedValue, result[theLocationId].First().Temperature);
    }

    [TestCase(86.0f, 30.0f)]
    [TestCase(104.0f, 40.0f)]
    [TestCase(122.0f, 50.0f)]
    [TestCase(140.0f, 60.0f)]
    public async Task GetLocationsWithTemperatureAboveAsync_WhenFahrenheitIsRequested_ConvertsFromCelsiusToFahrenheit(float expectedValue, float valueInCache)
    {
      var theLocationId = "aLocationId";
      var theLocation = new Location(theLocationId, new[]
      {
        new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), valueInCache, Unit.Celsius)
      });

      weatherDataCacheMock
        .TryGetLocationAsync(theLocationId)
        .Returns((true, theLocation));

      var result = await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Fahrenheit, 0.0f);
      Assert.AreEqual(expectedValue, result[theLocationId].First().Temperature);
    }

    [Test]
    public async Task GetFiveDayForecast_WhenCacheIsEmpty_DataIsRetrievedFromClientAndSavedToCache()
    {
      var theLocationId = "aLocationId";

      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((false, null));

      var upToDateLocation = new Location(theLocationId, new[] { new Forecast(DateTime.UtcNow, 0.0f, Unit.Celsius) });

      weatherDataClientMock
        .GetFiveDayForecastAsync(theLocationId)
        .Returns(upToDateLocation);

      await weatherService.GetFiveDayForecast(theLocationId);

      await weatherDataClientMock.Received(1).GetFiveDayForecastAsync(theLocationId);
      await weatherDataCacheMock.Received(1).UpdateAsync(Arg.Is<Location>(
        x => string.Equals(x.Id, upToDateLocation.Id) && x.Forecasts.First().Equals(upToDateLocation.Forecasts.First())));
    }

    [Test]
    public async Task GetFiveDayForecast_WhenCacheIsOutdated_DataIsRetrievedFromClientAndSavedToCache()
    {
      var theLocationId = "aLocationId";
      var locationInCache = new Location(theLocationId, new[] { new Forecast(DateTime.UtcNow, 0.0f, Unit.Celsius) });
      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((true, locationInCache));

      var upToDateLocation = new Location(theLocationId, new[] { new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), 0.0f, Unit.Celsius) });

      weatherDataClientMock
        .GetFiveDayForecastAsync(theLocationId)
        .Returns(upToDateLocation);

      await weatherService.GetLocationsWithTemperatureAboveAsync(new[] { theLocationId }, Unit.Celsius, 0.0f);

      await weatherDataClientMock.Received(1).GetFiveDayForecastAsync(theLocationId);
      await weatherDataCacheMock.Received(1).UpdateAsync(Arg.Is<Location>(
        x => string.Equals(x.Id, upToDateLocation.Id) && x.Forecasts.First().Equals(upToDateLocation.Forecasts.First())));
    }

    [Test]
    public async Task GetFiveDayForecast_WhenCacheIsValid_ClientIsNotCalled()
    {
      var theLocationId = "aLocationId";
      var theLocation = new Location(
          theLocationId, new[]
          {
            new Forecast(DateTime.UtcNow + TimeSpan.FromDays(1), 0.0f, Unit.Celsius)
          });

      weatherDataCacheMock.TryGetLocationAsync(theLocationId).Returns((true, theLocation));

      await weatherService.GetFiveDayForecast(theLocationId);

      await weatherDataClientMock.DidNotReceive().GetFiveDayForecastAsync(Arg.Any<string>());
    }
  }
}