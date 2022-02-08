using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.DependencyInjection;
using WeatherApi.Infrastructure.OpenWeatherMap;

namespace WeatherApi.Infrastructure
{
  public static class Module
  {
    public static IServiceCollection AddInfrastructureLayer(
      this IServiceCollection services,
      Action<InfrastructureLayerConfigurator> configure)
    {
      var configurator = new InfrastructureLayerConfigurator(services);
      configure.Invoke(configurator);

      services.AddSingleton<IWeatherDataClient, OpenWeatherMapClient>();

      var apiBaseUrl = configurator.WeatherApiBaseUrl ??
        throw new InvalidOperationException($"The {nameof(configurator.WeatherApiBaseUrl)} property of the configurator must be set");

      services.AddHttpClient<IWeatherDataClient, OpenWeatherMapClient>(x =>
      {
        x.BaseAddress = new Uri(apiBaseUrl);
      });

      if(!configurator.CacheSelected)
      {
        configurator.UseInMemoryCache();
      }

      return services;
    }
  }
}
