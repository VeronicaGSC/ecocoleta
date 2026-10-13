namespace EcoColeta.Api.Models;

/// <summary>
/// Contrato comum a todos os documentos persistidos no MongoDB.
/// </summary>
public interface IEntity
{
    string? Id { get; set; }
}
