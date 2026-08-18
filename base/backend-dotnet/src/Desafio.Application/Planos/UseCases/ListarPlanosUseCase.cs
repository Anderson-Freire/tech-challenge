using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.UseCases;

public sealed class ListarPlanosUseCase(
    IPlanoRepositorio repositorio,
    IValidator<ListarPlanosRequest> validador)
    : IUseCase<ListarPlanosRequest, IReadOnlyList<ListarPlanosResponse>>
{
    public async Task<IReadOnlyList<ListarPlanosResponse>> ExecutarAsync(
        ListarPlanosRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var planos = await repositorio.ListarAsync(cancellationToken);

        return [.. planos.Select(ListarPlanosResponse.De)];
    }
}
