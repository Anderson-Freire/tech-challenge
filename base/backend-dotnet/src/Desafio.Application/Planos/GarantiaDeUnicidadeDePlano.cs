using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Domain.Planos;

namespace Desafio.Application.Planos;

internal static class GarantiaDeUnicidadeDePlano
{
    public static async Task GarantirAsync(
        IPlanoRepositorio repositorio,
        Plano plano,
        CancellationToken cancellationToken)
    {
        var conflito = await repositorio.ObterConflitoAsync(
            plano.Id,
            plano.Nome,
            plano.CodigoRegistroAns.Valor,
            cancellationToken);

        if (conflito is null)
        {
            return;
        }

        var campo = string.Equals(conflito.Nome, plano.Nome, StringComparison.Ordinal)
            ? nameof(Plano.Nome)
            : nameof(Plano.CodigoRegistroAns);

        throw new ConflitoExcecao(
            "Já existe plano cadastrado com esse valor",
            [new DetalheErro(campo, "duplicado")]);
    }
}
