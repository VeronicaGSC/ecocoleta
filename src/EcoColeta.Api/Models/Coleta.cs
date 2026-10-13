using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

/// <summary>
/// Collection "coletas" (Ambiental + Governança): registro de cada coleta,
/// com peso por material e destino (rastreabilidade).
/// </summary>
[BsonIgnoreExtraElements]
public class Coleta : IEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("codigo")] public string Codigo { get; set; } = string.Empty;
    [BsonElement("ponto_coleta_codigo")] public string PontoColetaCodigo { get; set; } = string.Empty;
    [BsonElement("data_coleta")] public DateTime DataColeta { get; set; } = DateTime.UtcNow;
    [BsonElement("materiais")] public List<MaterialColetado> Materiais { get; set; } = new();
    [BsonElement("veiculo"), BsonIgnoreIfNull] public Veiculo? Veiculo { get; set; }
    [BsonElement("operador"), BsonIgnoreIfNull] public string? Operador { get; set; }
    [BsonElement("rota"), BsonIgnoreIfNull] public string? Rota { get; set; }
    [BsonElement("valor_reciclagem_estimado"), BsonIgnoreIfNull] public double? ValorReciclagemEstimado { get; set; }
    [BsonElement("incidentes"), BsonIgnoreIfNull] public List<Incidente>? Incidentes { get; set; }

    public double PesoTotalKg() => Math.Round(Materiais.Sum(m => m.PesoKg), 2);
}

[BsonIgnoreExtraElements]
public class MaterialColetado
{
    [BsonElement("tipo")] public string Tipo { get; set; } = string.Empty;
    [BsonElement("peso_kg")] public double PesoKg { get; set; }
    [BsonElement("destino")] public string Destino { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class Veiculo
{
    [BsonElement("placa")] public string Placa { get; set; } = string.Empty;
    [BsonElement("tipo")] public string Tipo { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class Incidente
{
    [BsonElement("descricao")] public string Descricao { get; set; } = string.Empty;
    [BsonElement("gravidade")] public string Gravidade { get; set; } = "baixa";
}
