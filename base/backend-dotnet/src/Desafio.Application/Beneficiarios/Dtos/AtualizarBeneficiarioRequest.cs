using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record AtualizarBeneficiarioRequest(
    Guid Id,
    string? NomeCompleto,
    DateOnly? DataNascimento,
    Guid? PlanoId,
    StatusBeneficiario? Status);
