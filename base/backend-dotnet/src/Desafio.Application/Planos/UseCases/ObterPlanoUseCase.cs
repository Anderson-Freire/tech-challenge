using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Planos.UseCases;

public sealed class ObterPlanoUseCase(
    IPlanoRepositorio repositorio,
    IValidator<ObterPlanoRequest> validador)
    : IUseCase<ObterPlanoRequest, ObterPlanoResponse>
{
    public async Task<ObterPlanoResponse> ExecutarAsync(
        ObterPlanoRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var plano = await repositorio
            .ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Plano não encontrado");

        return ObterPlanoResponse.De(plano);
    }
}
