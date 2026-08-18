using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using FluentValidation;

namespace Desafio.Api.Modules.Planos.Aplicacao.Validadores;

public sealed class AtualizarPlanoRequestValidador
    : AbstractValidator<AtualizarPlanoRequest>
{
    public AtualizarPlanoRequestValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Length(3, 60)
            .WithErrorCode("tamanho_invalido");

        RuleFor(x => x.CodigoRegistroAns)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Matches("^[0-9]{6}$")
            .WithErrorCode("formato_invalido")
            .When(x => !string.IsNullOrWhiteSpace(x.CodigoRegistroAns));
    }
}