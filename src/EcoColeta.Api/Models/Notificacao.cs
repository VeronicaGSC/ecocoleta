using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoColeta.Api.Models;

/// <summary>
/// Collection "notificacoes" (Social + Governança). O subdocumento "contexto"
/// muda conforme o tipo (alerta guarda ponto/nível; orientação guarda material/instrução).
/// </summary>
[BsonIgnoreExtraElements]
public class Notificacao : IEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("usuario_email")] public string UsuarioEmail { get; set; } = string.Empty;
    [BsonElement("tipo")] public string Tipo { get; set; } = TipoNotificacao.OrientacaoDescarte;
    [BsonElement("titulo")] public string Titulo { get; set; } = string.Empty;
    [BsonElement("mensagem")] public string Mensagem { get; set; } = string.Empty;
    [BsonElement("canal")] public string Canal { get; set; } = "push";
    [BsonElement("lida")] public bool Lida { get; set; }
    [BsonElement("data_envio")] public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    [BsonElement("contexto"), BsonIgnoreIfNull] public ContextoNotificacao? Contexto { get; set; }
}

[BsonIgnoreExtraElements]
public class ContextoNotificacao
{
    [BsonElement("ponto_codigo"), BsonIgnoreIfNull] public string? PontoCodigo { get; set; }
    [BsonElement("nivel_percent"), BsonIgnoreIfNull] public int? NivelPercent { get; set; }
    [BsonElement("material"), BsonIgnoreIfNull] public string? Material { get; set; }
    [BsonElement("instrucao"), BsonIgnoreIfNull] public string? Instrucao { get; set; }
    [BsonElement("data_agendada"), BsonIgnoreIfNull] public DateTime? DataAgendada { get; set; }
    [BsonElement("pontos_ganhos"), BsonIgnoreIfNull] public int? PontosGanhos { get; set; }
}

public static class TipoNotificacao
{
    public const string AlertaLimite = "alerta_limite";
    public const string OrientacaoDescarte = "orientacao_descarte";
    public const string ColetaAgendada = "coleta_agendada";
    public const string Recompensa = "recompensa";

    public static readonly string[] Todos = { AlertaLimite, OrientacaoDescarte, ColetaAgendada, Recompensa };
}
