using Desafio.Domain.Beneficiarios;
using Desafio.Domain.Compartilhado.Excecoes;

namespace Desafio.Application.Planos;

internal static class GarantiaDePlanoExistente
{
    public static async Task GarantirAsync(
        IPlanoRepositorio repositorio,
        Guid planoId,
        CancellationToken cancellationToken)
    {
        var plano = await repositorio.ObterPorIdAsync(planoId, cancellationToken);

        if (plano is null || plano.EstaExcluido)
        {
            throw new NaoProcessavelExcecao(
                "Plano informado não existe",
                [new DetalheErro(nameof(Beneficiario.PlanoId), "inexistente")]);
        }
    }
}
