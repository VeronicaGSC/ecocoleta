using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcoColeta.Api.Controllers;

/// <summary>Alertas e orientações enviados aos usuários (Social + Governança).</summary>
[Route("api/notificacoes")]
public class NotificacoesController : CrudController<Notificacao>
{
    public NotificacoesController(IRepository<Notificacao> repo) : base(repo) { }

    protected override List<string> Validar(Notificacao entidade) => Validador.ValidarNotificacao(entidade);

    /// <summary>Notificações ainda não lidas, das mais recentes para as mais antigas.</summary>
    [HttpGet("nao-lidas")]
    public async Task<ActionResult<List<Notificacao>>> NaoLidas(CancellationToken ct) =>
        Ok(await Repo.Collection.Find(n => !n.Lida)
                                .SortByDescending(n => n.DataEnvio)
                                .ToListAsync(ct));

    /// <summary>Marca uma notificação como lida.</summary>
    [HttpPatch("{id}/lida")]
    public async Task<IActionResult> MarcarComoLida(string id, CancellationToken ct)
    {
        var notificacao = await Repo.ObterAsync(id, ct);
        if (notificacao is null) return NotFound(new { mensagem = "Notificação não encontrada." });

        notificacao.Lida = true;
        await Repo.AtualizarAsync(id, notificacao, ct);
        return NoContent();
    }
}
