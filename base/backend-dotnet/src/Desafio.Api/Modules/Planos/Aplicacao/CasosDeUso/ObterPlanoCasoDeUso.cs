using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;

namespace Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;

public sealed class ObterPlanoCasoDeUso(
    IPlanoRepositorio repositorio)
    : ICasoDeUso<Guid, PlanoResponse>
{
    public async Task<PlanoResponse> ExecutarAsync(
        Guid entrada,
        CancellationToken cancellationToken)
    {
        var plano = await repositorio
            .ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Plano não encontrado");

        return PlanoMapeamento.ParaResponse(plano);
    }
}