namespace Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;

public sealed record AtualizarPlanoRequest(
    string? Nome,
    string? CodigoRegistroAns);