using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB;
public static class Extensions
{
  public static IServiceCollection AddMongo(this IServiceCollection services)
  {
    // Configure GUID serializer 
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

    // Configure MongoDB
    services.AddSingleton<IMongoClient>(serviceProvider =>
    {
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>() ?? new MongoDbSettings();
      return new MongoClient(mongoDbSettings.ConnectionString);
    });

    services.AddSingleton<IMongoDatabase>(serviceProvider =>
    {
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>() ?? new ServiceSettings();
      var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>() ?? new MongoDbSettings();
      var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
      return mongoClient.GetDatabase(serviceSettings.ServiceName);
    });

    return services;
  }

  public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
  {
    services.AddSingleton<IRepository<T>>(serviceProvider =>
    {
      // GetRequiredService to grantee that services will be available
      var database = serviceProvider.GetRequiredService<IMongoDatabase>();
      return new MongoRepository<T>(database, collectionName);
    });

    return services;
  }
}
