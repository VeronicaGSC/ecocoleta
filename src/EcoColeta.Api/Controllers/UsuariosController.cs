using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcoColeta.Api.Controllers;

/// <summary>Cidadãos e empresas (pilar Social) com gamificação.</summary>
[Route("api/usuarios")]
public class UsuariosController : CrudController<Usuario>
{
    public UsuariosController(IRepository<Usuario> repo) : base(repo) { }

    protected override List<string> Validar(Usuario entidade) => Validador.ValidarUsuario(entidade);

    /// <summary>Ranking dos 10 usuários com mais pontos de recompensa.</summary>
    [HttpGet("ranking")]
    public async Task<ActionResult<List<Usuario>>> Ranking(CancellationToken ct) =>
        Ok(await Repo.Collection.Find(FilterDefinition<Usuario>.Empty)
                                .SortByDescending(u => u.PontosRecompensa)
                                .Limit(10)
                                .ToListAsync(ct));

    /// <summary>Concede pontos por descarte correto e recalcula o nível de gamificação.</summary>
    [HttpPost("{id}/recompensas")]
    public async Task<IActionResult> Recompensar(string id, [FromBody] RecompensaRequest req, CancellationToken ct)
    {
        if (req.Pontos <= 0) return BadRequest(new { erros = new[] { "pontos deve ser maior que zero." } });

        var usuario = await Repo.ObterAsync(id, ct);
        if (usuario is null) return NotFound(new { mensagem = "Usuário não encontrado." });

        usuario.PontosRecompensa += req.Pontos;
        usuario.Gamificacao ??= new Gamificacao();
        usuario.Gamificacao.Nivel = GamificacaoService.CalcularNivel(usuario.PontosRecompensa);

        await Repo.AtualizarAsync(id, usuario, ct);
        return Ok(usuario);
    }
}
