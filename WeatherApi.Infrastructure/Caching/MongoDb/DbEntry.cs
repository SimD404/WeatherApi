using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WeatherApi.Domain;

namespace WeatherApi.Infrastructure.Caching.MongoDb
{
  internal class DbEntry
  {
    public DbEntry(Location location)
    {
      Id = location.Id;
      Location = location;
    }

    public string Id { get; }

    public Location Location { get; }
  }
}
