namespace Desafio.Application.Planos.Dtos;

public sealed record CriarPlanoRequest(
    string? Nome,
    string? CodigoRegistroAns);
