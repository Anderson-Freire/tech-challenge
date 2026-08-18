using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Compartilhado.Validacao;

public static class FluentValidatorExtensao
{
    public static async Task ValidarAsync<T>(
        this IValidator<T> validador,
        T instancia,
        CancellationToken cancellationToken)
    {
        var resultado = await validador.ValidateAsync(instancia, cancellationToken);

        if (resultado.IsValid)
        {
            return;
        }

        var detalhes = resultado.Errors
            .Select(erro => new DetalheErro(
                NomeDaPropriedade(erro.PropertyName),
                string.IsNullOrWhiteSpace(erro.ErrorCode) ? "invalido" : erro.ErrorCode))
            .ToList();

        throw new ValidacaoExcecao("Dados inválidos", detalhes);
    }

    private static string NomeDaPropriedade(string propriedade)
    {
        var ponto = propriedade.LastIndexOf('.');
        return ponto >= 0 ? propriedade[(ponto + 1)..] : propriedade;
    }
}
