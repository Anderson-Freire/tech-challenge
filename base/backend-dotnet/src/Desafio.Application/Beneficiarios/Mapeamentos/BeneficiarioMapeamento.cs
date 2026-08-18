using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios.Mapeamentos;

public static class BeneficiarioMapeamento
{
    public static Beneficiario ParaEntidade(CriarBeneficiarioRequest request) =>
        new(
            request.NomeCompleto,
            request.Cpf,
            request.DataNascimento!.Value,
            request.PlanoId!.Value);
}
