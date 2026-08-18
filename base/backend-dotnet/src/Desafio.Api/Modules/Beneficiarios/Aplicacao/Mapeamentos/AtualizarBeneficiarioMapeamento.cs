using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;

public static class AtualizarBeneficiarioMapeamento
{
    public static void Aplicar(
        Beneficiario beneficiario,
        AtualizarBeneficiarioRequest request)
    {
        beneficiario.Atualizar(
            request.NomeCompleto ?? string.Empty,
            request.DataNascimento,
            request.PlanoId,
            request.Status);
    }
}