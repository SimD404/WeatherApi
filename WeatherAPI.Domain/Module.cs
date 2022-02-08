using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Common;

namespace WeatherApi.Domain
{
  public static class Module
  {
    public static IServiceCollection AddDomainLayer(this IServiceCollection services)
    {
      services.AddSingleton<IWeatherService, WeatherService>();
      services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
      return services;
    }
  }
}
