using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Domain.Compartilhado.Validadores;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.Validadores;

public sealed class CriarBeneficiarioRequestValidador
    : AbstractValidator<CriarBeneficiarioRequest>
{
    public CriarBeneficiarioRequestValidador()
    {
        this.NomeCompleto(x => x.NomeCompleto);
        this.DataNascimento(x => x.DataNascimento);
        this.PlanoId(x => x.PlanoId);

        RuleFor(x => x.Cpf)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Must(ValidadorCpf.EhValido)
            .WithErrorCode("formato_invalido");
    }
}
