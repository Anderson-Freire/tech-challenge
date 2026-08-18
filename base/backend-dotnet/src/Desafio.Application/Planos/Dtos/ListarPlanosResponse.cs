using Desafio.Domain.Planos;

namespace Desafio.Application.Planos.Dtos;

public sealed record ListarPlanosResponse(
    Guid Id,
    string Nome,
    string CodigoRegistroAns)
{
    public static ListarPlanosResponse De(Plano plano) =>
        new(plano.Id, plano.Nome, plano.CodigoRegistroAns.Valor);
}
