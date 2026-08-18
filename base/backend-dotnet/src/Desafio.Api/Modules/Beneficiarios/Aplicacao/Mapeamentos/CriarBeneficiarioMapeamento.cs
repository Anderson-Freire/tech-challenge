using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;

public static class CriarBeneficiarioMapeamento
{
    public static Beneficiario ParaEntidade(
        CriarBeneficiarioRequest request) =>
        new(
            request.NomeCompleto ?? string.Empty,
            request.Cpf ?? string.Empty,
            request.DataNascimento,
            request.PlanoId);
}