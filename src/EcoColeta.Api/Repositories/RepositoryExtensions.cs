using EcoColeta.Api.Models;
using MongoDB.Driver;

namespace EcoColeta.Api.Repositories;

public static class RepositoryExtensions
{
    /// <summary>Registra um repositório Mongo para a collection informada.</summary>
    public static IServiceCollection AddRepositorio<T>(this IServiceCollection services, string nomeCollection)
        where T : class, IEntity
    {
        services.AddSingleton<IRepository<T>>(sp =>
            new MongoRepository<T>(sp.GetRequiredService<IMongoDatabase>(), nomeCollection));
        return services;
    }
}
