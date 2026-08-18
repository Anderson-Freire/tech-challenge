using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.UseCases;

public sealed class AtualizarBeneficiarioUseCase(
    IBeneficiarioRepositorio beneficiarioRepositorio,
    IPlanoRepositorio planoRepositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<AtualizarBeneficiarioRequest> validador)
    : IUseCase<AtualizarBeneficiarioRequest, AtualizarBeneficiarioResponse>
{
    public async Task<AtualizarBeneficiarioResponse> ExecutarAsync(
        AtualizarBeneficiarioRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var beneficiario = await beneficiarioRepositorio
            .ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Beneficiário não encontrado");

        var planoId = entrada.PlanoId!.Value;

        if (beneficiario.PlanoId != planoId)
        {
            await GarantiaDePlanoExistente.GarantirAsync(
                planoRepositorio,
                planoId,
                cancellationToken);
        }

        beneficiario.Atualizar(
            entrada.NomeCompleto,
            entrada.DataNascimento!.Value,
            planoId,
            entrada.Status!.Value);

        await unidadeDeTrabalho.SalvarAsync(cancellationToken);

        return AtualizarBeneficiarioResponse.De(beneficiario);
    }
}
