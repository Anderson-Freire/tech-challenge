using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado.Validacao;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

public sealed class ObterBeneficiarioRequestValidador
    : AbstractValidator<ObterBeneficiarioRequest>
{
    public ObterBeneficiarioRequestValidador()
    {
        this.Id(x => x.Id);
    }
}
