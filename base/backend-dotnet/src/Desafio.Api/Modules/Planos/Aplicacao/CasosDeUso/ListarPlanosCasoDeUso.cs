using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;

namespace Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;

public sealed class ListarPlanosCasoDeUso(
    IPlanoRepositorio repositorio)
    : ICasoDeUso<IReadOnlyList<PlanoResponse>>
{
    public async Task<IReadOnlyList<PlanoResponse>> ExecutarAsync(
        CancellationToken cancellationToken)
    {
        var planos = await repositorio.ListarAsync(cancellationToken);

        return PlanoMapeamento.ParaResponse(planos);
    }
}