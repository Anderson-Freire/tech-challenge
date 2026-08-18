using System.Linq.Expressions;
using Desafio.Domain.Planos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

internal static class RegrasDePlano
{
    public static void Nome<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, string?>> campo)
    {
        validador.RuleFor(campo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Length(LimitesDePlano.NomeMinimo, LimitesDePlano.NomeMaximo)
            .WithErrorCode("tamanho_invalido");
    }

    public static void CodigoRegistroAns<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, string?>> campo)
    {
        validador.RuleFor(campo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Matches("^[0-9]{6}$")
            .WithErrorCode("formato_invalido");
    }
}
