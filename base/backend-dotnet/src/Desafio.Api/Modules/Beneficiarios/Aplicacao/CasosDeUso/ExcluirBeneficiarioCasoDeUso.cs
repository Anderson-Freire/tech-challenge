using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;

public sealed class ExcluirBeneficiarioCasoDeUso(
    IBeneficiarioRepositorio repositorio)
    : ICasoDeUso<Guid, bool>
{
    public async Task<bool> ExecutarAsync(
        Guid entrada,
        CancellationToken cancellationToken)
    {
        var beneficiario = await repositorio
            .ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Beneficiário não encontrado");

        beneficiario.Excluir();

        await repositorio.SalvarAsync(cancellationToken);

        return true;
    }
}