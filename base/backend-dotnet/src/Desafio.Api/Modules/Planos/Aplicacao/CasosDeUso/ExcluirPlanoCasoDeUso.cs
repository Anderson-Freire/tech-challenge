

using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;

namespace Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;

public sealed class ExcluirPlanoUseCase(
    IPlanoRepositorio repositorio)
    : ICasoDeUso<Guid, bool>
{
    public async Task<bool> ExecutarAsync(
        Guid entrada,
        CancellationToken cancellationToken)
    {
        var plano = await repositorio
            .ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Plano não encontrado");

        plano.Excluir();

        await repositorio.SalvarAsync(cancellationToken);

        return true;
    }
}