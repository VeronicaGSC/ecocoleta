using EcoColeta.Api.Models;

namespace EcoColeta.Api.Services;

/// <summary>
/// Regras de alerta de capacidade dos pontos de coleta (pilar Ambiental).
/// </summary>
public static class AlertaService
{
    public const string EmailEquipeOperacional = "operacao@ecocoleta.app";

    /// <summary>Retorna os pontos que precisam de coleta, do mais cheio para o menos cheio.</summary>
    public static List<PontoColeta> FiltrarPontosEmAlerta(IEnumerable<PontoColeta> pontos) =>
        pontos.Where(p => p.PrecisaDeColeta())
              .OrderByDescending(p => p.NivelAtualPercent)
              .ToList();

    /// <summary>Gera a notificação de "limite atingido" para um ponto.</summary>
    public static Notificacao GerarNotificacaoAlerta(PontoColeta ponto, string emailDestino)
    {
        return new Notificacao
        {
            UsuarioEmail = emailDestino,
            Tipo = TipoNotificacao.AlertaLimite,
            Titulo = $"Coleta necessária: {ponto.Nome}",
            Mensagem = $"O ponto {ponto.Codigo} ({ponto.Nome}) atingiu {ponto.NivelAtualPercent}% da capacidade " +
                       $"(limite de alerta: {ponto.LimiteAlertaPercent}%). Agende a coleta.",
            Canal = "push",
            Lida = false,
            DataEnvio = DateTime.UtcNow,
            Contexto = new ContextoNotificacao
            {
                PontoCodigo = ponto.Codigo,
                NivelPercent = ponto.NivelAtualPercent
            }
        };
    }
}
