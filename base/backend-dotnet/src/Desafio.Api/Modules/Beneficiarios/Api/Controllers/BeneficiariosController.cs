using Desafio.Api.Api.Comum;
using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Validadores;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Modules.Beneficiarios.Api.Controllers;

[ApiController]
[Route("beneficiarios")]
[Produces("application/json")]
public sealed class BeneficiariosController(
    ListarBeneficiariosCasoDeUso listar,
    ObterBeneficiarioCasoDeUso obter,
    CriarBeneficiarioCasoDeUso criar,
    AtualizarBeneficiarioCasoDeUso atualizar,
    ExcluirBeneficiarioCasoDeUso excluir,
    IValidator<CriarBeneficiarioRequest> criarValidador,
    IValidator<AtualizarBeneficiarioRequest> atualizarValidador,
    IValidator<FiltroListarBeneficiariosRequest> filtroValidador)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<Resposta<ResultadoPaginado<BeneficiarioResponse>>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<Resposta<ResultadoPaginado<BeneficiarioResponse>>>(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] FiltroListarBeneficiariosRequest request,
        CancellationToken cancellationToken)
    {
        await filtroValidador.ValidarAsync(
            request,
            cancellationToken);

        var resultado = await listar.ExecutarAsync(
            request,
            cancellationToken);

        return Retornar(
            Resposta<ResultadoPaginado<BeneficiarioResponse>>.Ok(resultado));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(
        Guid id,
        CancellationToken cancellationToken)
    {
        var beneficiario = await obter.ExecutarAsync(
            id,
            cancellationToken);

        return Retornar(
            Resposta<BeneficiarioResponse>.Ok(beneficiario));
    }

    [HttpPost]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status201Created)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status409Conflict)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarBeneficiarioRequest request,
        CancellationToken cancellationToken)
    {
        await criarValidador.ValidarAsync(
            request,
            cancellationToken);

        var beneficiario = await criar.ExecutarAsync(
            request,
            cancellationToken);

        return Retornar(
            Resposta<BeneficiarioResponse>.Ok(
                beneficiario,
                System.Net.HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status409Conflict)]
    [ProducesResponseType<Resposta<BeneficiarioResponse>>(
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarBeneficiarioRequest request,
        CancellationToken cancellationToken)
    {
        await atualizarValidador.ValidarAsync(
            request,
            cancellationToken);

        var beneficiario = await atualizar.ExecutarAsync(
            new AtualizarBeneficiarioEntrada(id, request),
            cancellationToken);

        return Retornar(
            Resposta<BeneficiarioResponse>.Ok(beneficiario));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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