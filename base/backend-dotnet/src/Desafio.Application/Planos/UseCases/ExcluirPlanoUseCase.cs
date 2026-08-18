using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Planos.UseCases;

public sealed class ExcluirPlanoUseCase(
    IPlanoRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<ExcluirPlanoRequest> validador)
    : IUseCaseSemSaida<ExcluirPlanoRequest>
{
    public async Task ExecutarAsync(
        ExcluirPlanoRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var plano = await repositorio.ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Plano não encontrado");

        plano.Excluir();
        await unidadeDeTrabalho.SalvarAsync(cancellationToken);
    }
}
