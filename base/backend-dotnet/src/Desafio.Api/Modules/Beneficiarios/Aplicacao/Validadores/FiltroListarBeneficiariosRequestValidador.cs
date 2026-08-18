using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;
using FluentValidation;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Validadores;

public sealed class FiltroListarBeneficiariosRequestValidador
    : AbstractValidator<FiltroListarBeneficiariosRequest>
{
    public FiltroListarBeneficiariosRequestValidador()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode("minimo_1");

        RuleFor(x => x.Tamanho)
            .InclusiveBetween(1, 100)
            .WithErrorCode("entre_1_e_100");

        RuleFor(x => x.Status)
            .Must(status =>
                string.IsNullOrWhiteSpace(status) ||
                Enum.TryParse<StatusBeneficiario>(
                    status,
                    ignoreCase: true,
                    out _))
            .WithErrorCode("invalido");
    }
}