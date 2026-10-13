using EcoColeta.Api.Models;
using MongoDB.Driver;

namespace EcoColeta.Api.Repositories;

/// <summary>
/// Repositório genérico de CRUD sobre uma collection do MongoDB.
/// </summary>
public interface IRepository<T> where T : class, IEntity
{
    IMongoCollection<T> Collection { get; }
    Task<List<T>> ListarAsync(CancellationToken ct = default);
    Task<T?> ObterAsync(string id, CancellationToken ct = default);
    Task<T> CriarAsync(T entidade, CancellationToken ct = default);
    Task<bool> AtualizarAsync(string id, T entidade, CancellationToken ct = default);
    Task<bool> RemoverAsync(string id, CancellationToken ct = default);
}
