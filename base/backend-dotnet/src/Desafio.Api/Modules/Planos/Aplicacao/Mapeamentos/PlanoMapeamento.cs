using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;

public static class PlanoMapeamento
{
    public static PlanoResponse ParaResponse(Plano plano) =>
        new(
            plano.Id,
            plano.Nome,
            plano.CodigoRegistroAns.Valor);
    public static IReadOnlyList<PlanoResponse> ParaResponse(
        IEnumerable<Plano> planos) =>
        [.. planos.Select(ParaResponse)];
}