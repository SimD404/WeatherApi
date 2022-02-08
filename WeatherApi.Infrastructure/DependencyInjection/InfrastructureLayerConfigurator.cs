using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Caching;
using WeatherApi.Infrastructure.Caching.MongoDb;

namespace WeatherApi.Infrastructure.DependencyInjection
{
  public class InfrastructureLayerConfigurator
  {
    private readonly IServiceCollection services;
    private bool cacheSelected;

    internal InfrastructureLayerConfigurator(IServiceCollection services)
    {
      this.services = services;
    }

    public string WeatherApiBaseUrl { get; set; }
    internal bool CacheSelected => cacheSelected;

    public void UseInMemoryCache()
    {
      EnforceSingleCacheSelection();

      services.AddSingleton<IWeatherDataCache, InMemoryWeatherDataCache>();
    }

    public void UseMongoDbCache(Action<MongoDbCacheConfigurator> configure = null)
    {
      EnforceSingleCacheSelection();

      var configurator = new MongoDbCacheConfigurator(services);

      configure?.Invoke(configurator);

      var mongoClient = new MongoClient(configurator.ConnectionString);

      services.AddSingleton<IWeatherDataCache>(new MongoDbDataCache(mongoClient));
    }

    private void EnforceSingleCacheSelection()
    {
      if (cacheSelected)
      {
        throw new InvalidOperationException("The cache can only be configured once");
      }

      cacheSelected = true;
    }
  }
}