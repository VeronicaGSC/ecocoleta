using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

/// <summary>Indicadores ESG consolidados.</summary>
[ApiController]
[Route("api/relatorios")]
[Produces("application/json")]
public class RelatoriosController : ControllerBase
{
    private readonly IRepository<Coleta> _coletas;
    private readonly IRepository<MaterialReciclavel> _materiais;
    private readonly IRepository<PontoColeta> _pontos;

    public RelatoriosController(IRepository<Coleta> coletas, IRepository<MaterialReciclavel> materiais, IRepository<PontoColeta> pontos)
    {
        _coletas = coletas;
        _materiais = materiais;
        _pontos = pontos;
    }

    /// <summary>Relatório ESG: peso reciclado por material, CO2 evitado, água economizada e pontos em alerta.</summary>
    [HttpGet("esg")]
    public async Task<ActionResult<RelatorioEsg>> Esg(CancellationToken ct)
    {
        var coletas = await _coletas.ListarAsync(ct);
        var materiais = await _materiais.ListarAsync(ct);
        var pontos = await _pontos.ListarAsync(ct);
        return Ok(RelatorioService.GerarRelatorio(coletas, materiais, pontos));
    }
}
