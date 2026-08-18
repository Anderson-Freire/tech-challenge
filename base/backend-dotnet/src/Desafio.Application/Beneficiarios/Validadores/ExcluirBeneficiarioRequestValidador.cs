using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado.Validacao;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

public sealed class ExcluirBeneficiarioRequestValidador
    : AbstractValidator<ExcluirBeneficiarioRequest>
{
    public ExcluirBeneficiarioRequestValidador()
    {
        this.Id(x => x.Id);
    }
}
