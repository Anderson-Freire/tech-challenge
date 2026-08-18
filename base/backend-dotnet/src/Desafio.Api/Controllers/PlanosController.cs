using Desafio.Api.Contratos;
using Desafio.Application.Planos.Dtos;
using Desafio.Application.Planos.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Controllers;

[ApiController]
[Route("planos")]
[Produces("application/json")]
public sealed class PlanosController(
    ListarPlanosUseCase listar,
    ObterPlanoUseCase obter,
    CriarPlanoUseCase criar,
    AtualizarPlanoUseCase atualizar,
    ExcluirPlanoUseCase excluir) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ListarPlanosResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync(CancellationToken cancellationToken)
    {
        return Ok(await listar.ExecutarAsync(new ListarPlanosRequest(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ObterPlanoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await obter.ExecutarAsync(new ObterPlanoRequest(id), cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<CriarPlanoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarAsync(
        [FromBody] CriarPlanoRequest request,
        CancellationToken cancellationToken)
    {
        var plano = await criar.ExecutarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterAsync), new { id = plano.Id }, plano);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<AtualizarPlanoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AtualizarAsync(
        Guid id,
        [FromBody] AtualizarPlanoRequest request,
        CancellationToken cancellationToken)
    {
        var plano = await atualizar.ExecutarAsync(
            request with { Id = id },
            cancellationToken);

        return Ok(plano);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        await excluir.ExecutarAsync(new ExcluirPlanoRequest(id), cancellationToken);
        return NoContent();
    }
}
