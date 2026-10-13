using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcoColeta.Api.Controllers;

/// <summary>Catálogo de materiais recicláveis e destinação correta.</summary>
[Route("api/materiais")]
public class MateriaisController : CrudController<MaterialReciclavel>
{
    public MateriaisController(IRepository<MaterialReciclavel> repo) : base(repo) { }

    protected override List<string> Validar(MaterialReciclavel entidade) => Validador.ValidarMaterial(entidade);

    /// <summary>Orientação de descarte correto para uma categoria (ex.: pilha_bateria).</summary>
    [HttpGet("categoria/{categoria}")]
    public async Task<IActionResult> PorCategoria(string categoria, CancellationToken ct)
    {
        var material = await Repo.Collection.Find(m => m.Categoria == categoria).FirstOrDefaultAsync(ct);
        return material is null ? NotFound(new { mensagem = "Categoria não encontrada." }) : Ok(material);
    }
}
