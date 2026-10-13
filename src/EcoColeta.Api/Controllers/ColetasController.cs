using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcoColeta.Api.Controllers;

/// <summary>Registros de coleta (Ambiental + Governança / rastreabilidade).</summary>
[Route("api/coletas")]
public class ColetasController : CrudController<Coleta>
{
    public ColetasController(IRepository<Coleta> repo) : base(repo) { }

    protected override List<string> Validar(Coleta entidade) => Validador.ValidarColeta(entidade);

    /// <summary>Histórico de coletas de um ponto (rastreabilidade).</summary>
    [HttpGet("ponto/{codigoPonto}")]
    public async Task<ActionResult<List<Coleta>>> PorPonto(string codigoPonto, CancellationToken ct) =>
        Ok(await Repo.Collection.Find(c => c.PontoColetaCodigo == codigoPonto)
                                .SortByDescending(c => c.DataColeta)
                                .ToListAsync(ct));
}
