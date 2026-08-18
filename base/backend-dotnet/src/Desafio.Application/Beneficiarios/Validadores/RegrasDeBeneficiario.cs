using System.Linq.Expressions;
using Desafio.Domain.Beneficiarios;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

internal static class RegrasDeBeneficiario
{
    public static void NomeCompleto<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, string?>> campo)
    {
        validador.RuleFor(campo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Length(LimitesDeBeneficiario.NomeMinimo, LimitesDeBeneficiario.NomeMaximo)
            .WithErrorCode("tamanho_invalido");
    }

    public static void DataNascimento<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, DateOnly?>> campo)
    {
        validador.RuleFor(campo)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode("obrigatorio")
            .Must(data => data!.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode("deve_ser_passada");
    }

    public static void PlanoId<T>(
        this AbstractValidator<T> validador,
        Expression<Func<T, Guid?>> campo)
    {
        validador.RuleFor(campo)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode("obrigatorio")
            .Must(id => id != Guid.Empty)
            .WithErrorCode("obrigatorio");
    }
}
