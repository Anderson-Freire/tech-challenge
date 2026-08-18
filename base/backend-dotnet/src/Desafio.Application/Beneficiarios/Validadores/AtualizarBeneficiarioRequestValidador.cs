using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado.Validacao;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

public sealed class AtualizarBeneficiarioRequestValidador
    : AbstractValidator<AtualizarBeneficiarioRequest>
{
    public AtualizarBeneficiarioRequestValidador()
    {
        this.Id(x => x.Id);
        this.NomeCompleto(x => x.NomeCompleto);
        this.DataNascimento(x => x.DataNascimento);
        this.PlanoId(x => x.PlanoId);

        RuleFor(x => x.Status)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode("obrigatorio")
            .IsInEnum()
            .WithErrorCode("invalido");
    }
}
