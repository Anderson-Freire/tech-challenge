using Desafio.Api.Api.Comum;
using Desafio.Api.Kernel.Dominio.Validadores;
using Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Modules.Planos.Api.Controllers;

[ApiController]
[Route("planos")]
[Produces("application/json")]
public sealed class PlanosController(
    ListarPlanosCasoDeUso listar,
    ObterPlanoCasoDeUso obter,
    CriarPlanoCasoDeUso criar,
    AtualizarPlanoCasoDeUso atualizar,
    ExcluirPlanoUseCase excluir,
    IValidator<CriarPlanoRequest> criarValidador,
    IValidator<AtualizarPlanoRequest> atualizarValidador) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<Resposta<IReadOnlyList<PlanoResponse>>>(
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        CancellationToken cancellationToken)
    {
        var planos = await listar.ExecutarAsync(cancellationToken);

        return Retornar(
            Resposta<IReadOnlyList<PlanoResponse>>.Ok(planos));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Resposta<PlanoResponse>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(
        Guid id,
        CancellationToken cancellationToken)
    {
        var plano = await obter.ExecutarAsync(
            id,
            cancellationToken);

        return Retornar(
            Resposta<PlanoResponse>.Ok(plano));
    }

    [HttpPost]
    [ProducesResponseType<Resposta<PlanoResponse>>(
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarPlanoRequest request,
        CancellationToken cancellationToken)
    {
        await criarValidador.ValidarAsync(
            request,
            cancellationToken);

        var plano = await criar.ExecutarAsync(
            request,
            cancellationToken);

        return Retornar(
            Resposta<PlanoResponse>.Ok(
                plano,
                System.Net.HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<Resposta<PlanoResponse>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarPlanoRequest request,
        CancellationToken cancellationToken)
    {
        await atualizarValidador.ValidarAsync(
            request,
            cancellationToken);

        var plano = await atualizar.ExecutarAsync(
            new AtualizarPlanoEntrada(id, request),
            cancellationToken);

        return Retornar(
            Resposta<PlanoResponse>.Ok(plano));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        Guid id,
        CancellationToken cancellationToken)
    {
        await excluir.ExecutarAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    private ObjectResult Retornar<T>(Resposta<T> resposta) =>
        StatusCode(
            (int)resposta.CodigoStatus,
            resposta);
}