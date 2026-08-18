using Desafio.Api.Contratos;
using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Beneficiarios.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Controllers;

[ApiController]
[Route("beneficiarios")]
[Produces("application/json")]
public sealed class BeneficiariosController(
    ListarBeneficiariosUseCase listar,
    ObterBeneficiarioUseCase obter,
    CriarBeneficiarioUseCase criar,
    AtualizarBeneficiarioUseCase atualizar,
    ExcluirBeneficiarioUseCase excluir) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ListarBeneficiariosResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarAsync(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 10,
        [FromQuery] string? status = null,
        [FromQuery(Name = "plano_id")] Guid? planoId = null,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listar.ExecutarAsync(
            new ListarBeneficiariosRequest(pagina, tamanho, status, planoId),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ObterBeneficiarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await obter.ExecutarAsync(new ObterBeneficiarioRequest(id), cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<CriarBeneficiarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CriarAsync(
        [FromBody] CriarBeneficiarioRequest request,
        CancellationToken cancellationToken)
    {
        var beneficiario = await criar.ExecutarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterAsync), new { id = beneficiario.Id }, beneficiario);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<AtualizarBeneficiarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AtualizarAsync(
        Guid id,
        [FromBody] AtualizarBeneficiarioRequest request,
        CancellationToken cancellationToken)
    {
        var beneficiario = await atualizar.ExecutarAsync(
            request with { Id = id },
            cancellationToken);

        return Ok(beneficiario);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        await excluir.ExecutarAsync(new ExcluirBeneficiarioRequest(id), cancellationToken);
        return NoContent();
    }
}
