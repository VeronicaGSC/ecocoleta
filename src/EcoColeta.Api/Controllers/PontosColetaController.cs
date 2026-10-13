using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

/// <summary>Pontos de coleta (pilar Ambiental) e alertas de capacidade.</summary>
[Route("api/pontos-coleta")]
public class PontosColetaController : CrudController<PontoColeta>
{
    private readonly IRepository<Notificacao> _notificacoes;

    public PontosColetaController(IRepository<PontoColeta> repo, IRepository<Notificacao> notificacoes) : base(repo)
    {
        _notificacoes = notificacoes;
    }

    protected override List<string> Validar(PontoColeta entidade) => Validador.ValidarPontoColeta(entidade);

    /// <summary>Lista os pontos que atingiram o limite e precisam de coleta.</summary>
    [HttpGet("alertas")]
    public async Task<ActionResult<List<PontoColeta>>> Alertas(CancellationToken ct) =>
        Ok(AlertaService.FiltrarPontosEmAlerta(await Repo.ListarAsync(ct)));

    /// <summary>
    /// Registra uma leitura do sensor IoT. Se o ponto atingir o limite,
    /// gera automaticamente uma notificação de alerta para a equipe operacional.
    /// </summary>
    [HttpPost("{id}/leituras")]
    public async Task<IActionResult> RegistrarLeitura(string id, [FromBody] LeituraSensorRequest leitura, CancellationToken ct)
    {
        if (leitura.NivelAtualPercent is < 0 or > 100)
            return BadRequest(new { erros = new[] { "nivelAtualPercent deve estar entre 0 e 100." } });

        var ponto = await Repo.ObterAsync(id, ct);
        if (ponto is null) return NotFound(new { mensagem = "Ponto de coleta não encontrado." });

        ponto.NivelAtualPercent = leitura.NivelAtualPercent;
        if (ponto.SensorIot is not null) ponto.SensorIot.UltimaLeitura = DateTime.UtcNow;

        Notificacao? alerta = null;
        if (ponto.PrecisaDeColeta())
        {
            alerta = AlertaService.GerarNotificacaoAlerta(ponto, AlertaService.EmailEquipeOperacional);
            await _notificacoes.CriarAsync(alerta, ct);
        }

        await Repo.AtualizarAsync(id, ponto, ct);
        return Ok(new { ponto, alertaGerado = alerta is not null, alerta });
    }
}
