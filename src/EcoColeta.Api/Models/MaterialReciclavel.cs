using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

/// <summary>
/// Collection "materiais_reciclaveis" (Ambiental + Social): catálogo com cor da
/// coleta seletiva, instruções de descarte e impacto ambiental.
/// </summary>
[BsonIgnoreExtraElements]
public class MaterialReciclavel : IEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")] public string Nome { get; set; } = string.Empty;
    [BsonElement("categoria")] public string Categoria { get; set; } = string.Empty;
    [BsonElement("reciclavel")] public bool Reciclavel { get; set; } = true;
    [BsonElement("cor_coleta_seletiva")] public string CorColetaSeletiva { get; set; } = string.Empty;
    [BsonElement("instrucoes_descarte")] public string InstrucoesDescarte { get; set; } = string.Empty;
    [BsonElement("tempo_decomposicao"), BsonIgnoreIfNull] public string? TempoDecomposicao { get; set; }
    [BsonElement("valor_kg_estimado")] public double ValorKgEstimado { get; set; }
    [BsonElement("impacto_ambiental"), BsonIgnoreIfNull] public ImpactoAmbiental? ImpactoAmbiental { get; set; }
    [BsonElement("alerta_especial"), BsonIgnoreIfNull] public string? AlertaEspecial { get; set; }
    [BsonElement("subtipos"), BsonIgnoreIfNull] public List<string>? Subtipos { get; set; }
}

[BsonIgnoreExtraElements]
public class ImpactoAmbiental
{
    [BsonElement("co2_evitado_kg_por_kg")] public double Co2EvitadoKgPorKg { get; set; }
    [BsonElement("agua_economizada_litros")] public double AguaEconomizadaLitros { get; set; }
}
