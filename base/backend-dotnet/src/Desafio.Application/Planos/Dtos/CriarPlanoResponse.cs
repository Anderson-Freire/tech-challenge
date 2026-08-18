using Desafio.Domain.Planos;

namespace Desafio.Application.Planos.Dtos;

public sealed record CriarPlanoResponse(
    Guid Id,
    string Nome,
    string CodigoRegistroAns)
{
    public static CriarPlanoResponse De(Plano plano) =>
        new(plano.Id, plano.Nome, plano.CodigoRegistroAns.Valor);
}
