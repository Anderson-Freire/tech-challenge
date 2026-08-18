
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;

public static class BeneficiarioMapeamento
{
    public static Beneficiario ParaEntidade(CriarBeneficiarioRequest request) =>
        new(
            request.NomeCompleto ?? string.Empty,
            request.Cpf ?? string.Empty,
            request.DataNascimento,
            request.PlanoId);

    public static BeneficiarioResponse ParaResponse(Beneficiario beneficiario) =>
        new(
            beneficiario.Id,
            beneficiario.NomeCompleto,
            beneficiario.Cpf.Valor,
            beneficiario.DataNascimento,
            beneficiario.Status,
            beneficiario.PlanoId,
            beneficiario.DataCadastro);

    public static IReadOnlyList<BeneficiarioResponse> ParaResponse(
        IEnumerable<Beneficiario> beneficiarios) =>
        [.. beneficiarios.Select(ParaResponse)];
}