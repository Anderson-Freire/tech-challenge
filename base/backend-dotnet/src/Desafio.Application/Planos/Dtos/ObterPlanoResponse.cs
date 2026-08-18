using Desafio.Domain.Planos;

namespace Desafio.Application.Planos.Dtos;

public sealed record ObterPlanoResponse(
    Guid Id,
    string Nome,
    string CodigoRegistroAns)
{
    public static ObterPlanoResponse De(Plano plano) =>
        new(plano.Id, plano.Nome, plano.CodigoRegistroAns.Valor);
}
