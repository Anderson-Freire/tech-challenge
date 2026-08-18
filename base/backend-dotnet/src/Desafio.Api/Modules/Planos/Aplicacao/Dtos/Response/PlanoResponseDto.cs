namespace Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;

public sealed record PlanoResponse(
    Guid Id,
    string Nome,
    string CodigoRegistroAns);