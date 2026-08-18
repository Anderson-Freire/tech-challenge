using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.UseCases;

public sealed class ObterBeneficiarioUseCase(
    IBeneficiarioRepositorio repositorio,
    IValidator<ObterBeneficiarioRequest> validador)
    : IUseCase<ObterBeneficiarioRequest, ObterBeneficiarioResponse>
{
    public async Task<ObterBeneficiarioResponse> ExecutarAsync(
        ObterBeneficiarioRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var beneficiario = await repositorio.ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Beneficiário não encontrado");

        return ObterBeneficiarioResponse.De(beneficiario);
    }
}
