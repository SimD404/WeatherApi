using Microsoft.Extensions.DependencyInjection;

namespace WeatherApi.Infrastructure.DependencyInjection
{
  public class MongoDbCacheConfigurator
  {
    private readonly IServiceCollection services;

    internal MongoDbCacheConfigurator(IServiceCollection services)
    {
      this.services = services;
    }

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
  }
}
