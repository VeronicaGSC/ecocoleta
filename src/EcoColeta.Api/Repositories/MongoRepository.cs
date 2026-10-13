using EcoColeta.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EcoColeta.Api.Repositories;

public class MongoRepository<T> : IRepository<T> where T : class, IEntity
{
    public IMongoCollection<T> Collection { get; }

    public MongoRepository(IMongoDatabase database, string nomeCollection)
    {
        Collection = database.GetCollection<T>(nomeCollection);
    }

    public Task<List<T>> ListarAsync(CancellationToken ct = default) =>
        Collection.Find(FilterDefinition<T>.Empty).ToListAsync(ct);

    public async Task<T?> ObterAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return null;
        return await Collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync(ct);
    }

    public async Task<T> CriarAsync(T entidade, CancellationToken ct = default)
    {
        entidade.Id = null; // o MongoDB gera o ObjectId
        await Collection.InsertOneAsync(entidade, cancellationToken: ct);
        return entidade;
    }

    public async Task<bool> AtualizarAsync(string id, T entidade, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return false;
        entidade.Id = id;
        var resultado = await Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", objectId), entidade, cancellationToken: ct);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return false;
        var resultado = await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", objectId), ct);
        return resultado.DeletedCount > 0;
    }
}
