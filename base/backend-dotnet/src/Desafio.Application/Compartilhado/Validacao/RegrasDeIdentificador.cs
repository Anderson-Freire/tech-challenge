using System.Linq.Expressions;
using FluentValidation;

namespace Desafio.Application.Compartilhado.Validacao;

internal static class RegrasDeIdentificador
{
    public static void Id<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, Guid>> campo)
    {
        validador.RuleFor(campo)
            .NotEmpty()
            .WithErrorCode("obrigatorio");
    }
}
