using MongoDB.Bson;
using MongoDB.Driver;
using WeatherApi.Domain;

namespace WeatherApi.Infrastructure.Caching.MongoDb
{
  internal class MongoDbDataCache : IWeatherDataCache
  {
    private readonly IMongoCollection<DbEntry> collection;
    public MongoDbDataCache(IMongoClient mongoClient)
    {
      var db = mongoClient.GetDatabase("weather-api");
      collection = db.GetCollection<DbEntry>("locations");
    }

    public async Task<(bool result, Location? location)> TryGetLocationAsync(string id)
    {
      var result = collection.Find(x => string.Equals(x.Id, id));
      return (result.Any(), (await result.SingleOrDefaultAsync())?.Location);
    }

    public async Task UpdateAsync(Location location)
    {
      var dbEntry = new DbEntry(location);
      await collection.ReplaceOneAsync(x => x.Id == dbEntry.Id, dbEntry, new ReplaceOptions { IsUpsert = true });
    }
  }
}
