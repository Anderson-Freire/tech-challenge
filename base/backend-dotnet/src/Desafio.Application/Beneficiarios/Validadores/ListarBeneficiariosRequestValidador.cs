using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Domain.Beneficiarios;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

public sealed class ListarBeneficiariosRequestValidador
    : AbstractValidator<ListarBeneficiariosRequest>
{
    public ListarBeneficiariosRequestValidador()
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
                Enum.TryParse<StatusBeneficiario>(status, ignoreCase: true, out _))
            .WithErrorCode("invalido");
    }
}
