namespace Desafio.Application.Planos.Dtos;

public sealed record AtualizarPlanoRequest(
    Guid Id,
    string? Nome,
    string? CodigoRegistroAns);
