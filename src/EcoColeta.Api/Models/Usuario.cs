using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

/// <summary>
/// Collection "usuarios" (pilar Social): cidadãos e empresas, com gamificação.
/// Esquema flexível: "individual" tem cpf; "empresa" tem cnpj, razao_social e contato.
/// </summary>
[BsonIgnoreExtraElements]
public class Usuario : IEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")] public string Nome { get; set; } = string.Empty;
    [BsonElement("email")] public string Email { get; set; } = string.Empty;
    [BsonElement("tipo")] public string Tipo { get; set; } = TipoUsuario.Individual;
    [BsonElement("cpf"), BsonIgnoreIfNull] public string? Cpf { get; set; }
    [BsonElement("cnpj"), BsonIgnoreIfNull] public string? Cnpj { get; set; }
    [BsonElement("razao_social"), BsonIgnoreIfNull] public string? RazaoSocial { get; set; }
    [BsonElement("contato"), BsonIgnoreIfNull] public Contato? Contato { get; set; }
    [BsonElement("endereco"), BsonIgnoreIfNull] public Endereco? Endereco { get; set; }
    [BsonElement("pontos_recompensa")] public int PontosRecompensa { get; set; }
    [BsonElement("gamificacao"), BsonIgnoreIfNull] public Gamificacao? Gamificacao { get; set; }
    [BsonElement("data_cadastro")] public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
public class Contato
{
    [BsonElement("responsavel")] public string Responsavel { get; set; } = string.Empty;
    [BsonElement("telefone")] public string Telefone { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class Gamificacao
{
    [BsonElement("nivel")] public string Nivel { get; set; } = string.Empty;
    [BsonElement("badges")] public List<string> Badges { get; set; } = new();
}

public static class TipoUsuario
{
    public const string Individual = "individual";
    public const string Empresa = "empresa";
}

/// <summary>Corpo da requisição de concessão de pontos.</summary>
public record RecompensaRequest(int Pontos, string? Motivo);
