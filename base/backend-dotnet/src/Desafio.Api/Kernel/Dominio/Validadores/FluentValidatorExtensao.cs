
using System.Text.Json;
using Desafio.Api.Kernel.Dominio.Excecoes;
using FluentValidation;

namespace Desafio.Api.Kernel.Dominio.Validadores;

public static class FluentValidationExtensao
{
    public static async Task ValidarAsync<T>(
        this IValidator<T> validador,
        T instancia,
        CancellationToken cancellationToken)
    {
        var resultado = await validador.ValidateAsync(
            instancia,
            cancellationToken);

        if (resultado.IsValid)
        {
            return;
        }

        var detalhes = resultado.Errors
            .Select(erro => new DetalheErro(
                JsonNamingPolicy.SnakeCaseLower.ConvertName(erro.PropertyName),
                erro.ErrorCode ?? "invalido"))
            .ToList();

        throw new ValidacaoExcecao(
            "Dados inválidos",
            detalhes);
    }
}