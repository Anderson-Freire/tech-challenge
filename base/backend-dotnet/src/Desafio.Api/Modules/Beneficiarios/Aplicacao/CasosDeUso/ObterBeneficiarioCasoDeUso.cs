using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;

public class ObterBeneficiarioCasoDeUso(
    IBeneficiarioRepositorio repositorio)
    : ICasoDeUso<Guid, BeneficiarioResponse>
{
    public async Task<BeneficiarioResponse> ExecutarAsync(
        Guid entrada,
        CancellationToken cancellationToken)
    {
        var beneficiario = await repositorio
            .ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Beneficiário não encontrado");

        return BeneficiarioMapeamento.ParaResponse(beneficiario);
    }
}