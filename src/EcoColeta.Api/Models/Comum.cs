using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

[BsonIgnoreExtraElements]
public class Endereco
{
    [BsonElement("logradouro"), BsonIgnoreIfNull] public string? Logradouro { get; set; }
    [BsonElement("bairro"), BsonIgnoreIfNull] public string? Bairro { get; set; }
    [BsonElement("cidade"), BsonIgnoreIfNull] public string? Cidade { get; set; }
    [BsonElement("uf"), BsonIgnoreIfNull] public string? Uf { get; set; }
    [BsonElement("cep"), BsonIgnoreIfNull] public string? Cep { get; set; }
}

/// <summary>
/// Ponto geográfico no formato GeoJSON (longitude, latitude).
/// </summary>
[BsonIgnoreExtraElements]
public class Localizacao
{
    [BsonElement("type")] public string Type { get; set; } = "Point";
    [BsonElement("coordinates")] public double[] Coordinates { get; set; } = Array.Empty<double>();
}
