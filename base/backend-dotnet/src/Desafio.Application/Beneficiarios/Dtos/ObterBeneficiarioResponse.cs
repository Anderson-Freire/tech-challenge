using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record ObterBeneficiarioResponse(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    StatusBeneficiario Status,
    Guid PlanoId,
    DateTime DataCadastro)
{
    public static ObterBeneficiarioResponse De(Beneficiario beneficiario) =>
        new(
            beneficiario.Id,
            beneficiario.NomeCompleto,
            beneficiario.Cpf.Valor,
            beneficiario.DataNascimento,
            beneficiario.Status,
            beneficiario.PlanoId,
            beneficiario.DataCadastro);
}
