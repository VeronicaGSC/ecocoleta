using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using MongoDB.Driver;

namespace EcoColeta.Api.Data;

/// <summary>
/// Cria índices e carrega os dados iniciais quando as collections estão vazias.
/// Idempotente: pode rodar a cada inicialização do contêiner.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        try
        {
            await CriarIndicesAsync(services);
            await PopularAsync(services.GetRequiredService<IRepository<PontoColeta>>(), DadosIniciais.PontosColeta(), logger);
            await PopularAsync(services.GetRequiredService<IRepository<Coleta>>(), DadosIniciais.Coletas(), logger);
            await PopularAsync(services.GetRequiredService<IRepository<Usuario>>(), DadosIniciais.Usuarios(), logger);
            await PopularAsync(services.GetRequiredService<IRepository<Notificacao>>(), DadosIniciais.Notificacoes(), logger);
            await PopularAsync(services.GetRequiredService<IRepository<MaterialReciclavel>>(), DadosIniciais.Materiais(), logger);
        }
        catch (Exception ex)
        {
            // A aplicação sobe mesmo sem banco; o /health indicará o problema.
            logger.LogWarning(ex, "Não foi possível popular o banco de dados na inicialização.");
        }
    }

    private static async Task PopularAsync<T>(IRepository<T> repo, List<T> dados, ILogger logger) where T : class, IEntity
    {
        var total = await repo.Collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
        if (total > 0) return;

        await repo.Collection.InsertManyAsync(dados);
        logger.LogInformation("Collection {Collection} populada com {Quantidade} documentos.",
            repo.Collection.CollectionNamespace.CollectionName, dados.Count);
    }

    private static async Task CriarIndicesAsync(IServiceProvider services)
    {
        var pontos = services.GetRequiredService<IRepository<PontoColeta>>().Collection;
        await pontos.Indexes.CreateOneAsync(new CreateIndexModel<PontoColeta>(
            Builders<PontoColeta>.IndexKeys.Ascending(p => p.Codigo),
            new CreateIndexOptions { Unique = true, Name = "ux_codigo" }));

        var usuarios = services.GetRequiredService<IRepository<Usuario>>().Collection;
        await usuarios.Indexes.CreateOneAsync(new CreateIndexModel<Usuario>(
            Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "ux_email" }));
    }
}
