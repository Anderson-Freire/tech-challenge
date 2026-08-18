using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record CriarBeneficiarioResponse(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    StatusBeneficiario Status,
    Guid PlanoId,
    DateTime DataCadastro)
{
    public static CriarBeneficiarioResponse De(Beneficiario beneficiario) =>
        new(
            beneficiario.Id,
            beneficiario.NomeCompleto,
            beneficiario.Cpf.Valor,
            beneficiario.DataNascimento,
            beneficiario.Status,
            beneficiario.PlanoId,
            beneficiario.DataCadastro);
}
