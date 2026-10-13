using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

/// <summary>
/// Collection "pontos_coleta" (pilar Ambiental): ecopontos e lixeiras inteligentes.
/// Esquema flexível: sensor_iot e energia_solar são opcionais.
/// </summary>
[BsonIgnoreExtraElements]
public class PontoColeta : IEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("codigo")] public string Codigo { get; set; } = string.Empty;
    [BsonElement("nome")] public string Nome { get; set; } = string.Empty;
    [BsonElement("tipo")] public string Tipo { get; set; } = "ecoponto";
    [BsonElement("endereco"), BsonIgnoreIfNull] public Endereco? Endereco { get; set; }
    [BsonElement("localizacao"), BsonIgnoreIfNull] public Localizacao? Localizacao { get; set; }
    [BsonElement("materiais_aceitos")] public List<string> MateriaisAceitos { get; set; } = new();
    [BsonElement("capacidade_litros")] public int CapacidadeLitros { get; set; }
    [BsonElement("nivel_atual_percent")] public int NivelAtualPercent { get; set; }
    [BsonElement("limite_alerta_percent")] public int LimiteAlertaPercent { get; set; } = 85;
    [BsonElement("status")] public string Status { get; set; } = StatusPonto.Ativo;
    [BsonElement("sensor_iot"), BsonIgnoreIfNull] public SensorIot? SensorIot { get; set; }
    [BsonElement("energia_solar"), BsonIgnoreIfNull] public bool? EnergiaSolar { get; set; }

    /// <summary>
    /// Regra de negócio central: o ponto precisa de coleta quando está ativo
    /// e o nível de enchimento alcançou ou superou o limite de alerta.
    /// </summary>
    public bool PrecisaDeColeta() =>
        Status == StatusPonto.Ativo && NivelAtualPercent >= LimiteAlertaPercent;
}

[BsonIgnoreExtraElements]
public class SensorIot
{
    [BsonElement("modelo")] public string Modelo { get; set; } = string.Empty;
    [BsonElement("bateria_percent"), BsonIgnoreIfNull] public int? BateriaPercent { get; set; }
    [BsonElement("ultima_leitura"), BsonIgnoreIfNull] public DateTime? UltimaLeitura { get; set; }
}

public static class StatusPonto
{
    public const string Ativo = "ativo";
    public const string ColetaAgendada = "coleta_agendada";
    public const string Manutencao = "manutencao";
    public const string Inativo = "inativo";

    public static readonly string[] Todos = { Ativo, ColetaAgendada, Manutencao, Inativo };
}

/// <summary>Corpo da requisição de leitura do sensor IoT.</summary>
public record LeituraSensorRequest(int NivelAtualPercent);
