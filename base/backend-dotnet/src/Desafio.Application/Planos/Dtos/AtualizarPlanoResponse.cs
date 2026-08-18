using Desafio.Domain.Planos;

namespace Desafio.Application.Planos.Dtos;

public sealed record AtualizarPlanoResponse(
    Guid Id,
    string Nome,
    string CodigoRegistroAns)
{
    public static AtualizarPlanoResponse De(Plano plano) =>
        new(plano.Id, plano.Nome, plano.CodigoRegistroAns.Valor);
}
