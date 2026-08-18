using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.UseCases;

public sealed class ExcluirBeneficiarioUseCase(
    IBeneficiarioRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<ExcluirBeneficiarioRequest> validador)
    : IUseCaseSemSaida<ExcluirBeneficiarioRequest>
{
    public async Task ExecutarAsync(
        ExcluirBeneficiarioRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var beneficiario = await repositorio.ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Beneficiário não encontrado");

        beneficiario.Excluir();
        await unidadeDeTrabalho.SalvarAsync(cancellationToken);
    }
}
