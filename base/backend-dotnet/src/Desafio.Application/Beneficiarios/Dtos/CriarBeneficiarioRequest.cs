namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record CriarBeneficiarioRequest(
    string? NomeCompleto,
    string? Cpf,
    DateOnly? DataNascimento,
    Guid? PlanoId);
