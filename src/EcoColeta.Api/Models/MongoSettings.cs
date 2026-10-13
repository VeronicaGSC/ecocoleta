namespace EcoColeta.Api.Models;

/// <summary>
/// Configurações do MongoDB. Em contêiner/Azure são lidas das variáveis
/// de ambiente Mongo__ConnectionString e Mongo__Database.
/// </summary>
public class MongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "ecocoleta";
}
