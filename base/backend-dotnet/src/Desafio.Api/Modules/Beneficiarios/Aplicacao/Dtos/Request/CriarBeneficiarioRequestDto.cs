namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;

public sealed record CriarBeneficiarioRequest(
    string? NomeCompleto,
    string? Cpf,
    DateOnly DataNascimento,
    Guid PlanoId);