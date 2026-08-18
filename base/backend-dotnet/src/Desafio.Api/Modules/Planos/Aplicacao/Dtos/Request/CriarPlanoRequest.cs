namespace Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;

public sealed record CriarPlanoRequest(
    string? Nome,
    string? CodigoRegistroAns);
