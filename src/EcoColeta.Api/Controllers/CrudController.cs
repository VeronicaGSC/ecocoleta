using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

/// <summary>
/// Controller base com as operações CRUD (GET, GET por id, POST, PUT, DELETE)
/// reaproveitadas pelas 5 collections.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class CrudController<T> : ControllerBase where T : class, IEntity
{
    protected readonly IRepository<T> Repo;

    protected CrudController(IRepository<T> repo) => Repo = repo;

    /// <summary>Validação específica de cada collection.</summary>
    protected abstract List<string> Validar(T entidade);

    [HttpGet]
    public async Task<ActionResult<List<T>>> Listar(CancellationToken ct) =>
        Ok(await Repo.ListarAsync(ct));

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<T>> Obter(string id, CancellationToken ct)
    {
        var entidade = await Repo.ObterAsync(id, ct);
        return entidade is null ? NotFound(new { mensagem = "Registro não encontrado." }) : Ok(entidade);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<T>> Criar([FromBody] T entidade, CancellationToken ct)
    {
        var erros = Validar(entidade);
        if (erros.Count > 0) return BadRequest(new { erros });

        var criado = await Repo.CriarAsync(entidade, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] T entidade, CancellationToken ct)
    {
        var erros = Validar(entidade);
        if (erros.Count > 0) return BadRequest(new { erros });

        return await Repo.AtualizarAsync(id, entidade, ct) ? NoContent() : NotFound(new { mensagem = "Registro não encontrado." });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id, CancellationToken ct) =>
        await Repo.RemoverAsync(id, ct) ? NoContent() : NotFound(new { mensagem = "Registro não encontrado." });
}
