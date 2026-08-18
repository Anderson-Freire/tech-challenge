using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record ListarBeneficiariosResponse(
    IReadOnlyList<ListarBeneficiariosItemResponse> Dados,
    int Pagina,
    int Tamanho,
    int Total);

public sealed record ListarBeneficiariosItemResponse(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    StatusBeneficiario Status,
    Guid PlanoId,
    DateTime DataCadastro)
{
    public static ListarBeneficiariosItemResponse De(Beneficiario beneficiario) =>
        new(
            beneficiario.Id,
            beneficiario.NomeCompleto,
            beneficiario.Cpf.Valor,
            beneficiario.DataNascimento,
            beneficiario.Status,
            beneficiario.PlanoId,
            beneficiario.DataCadastro);
}
