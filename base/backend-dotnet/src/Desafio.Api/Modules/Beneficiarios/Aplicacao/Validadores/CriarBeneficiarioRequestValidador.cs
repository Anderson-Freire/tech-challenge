using Desafio.Api.Kernel.Dominio.Validadores;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using FluentValidation;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Validadores;

public sealed class CriarBeneficiarioRequestValidador
    : AbstractValidator<CriarBeneficiarioRequest>
{
    public CriarBeneficiarioRequestValidador()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty()
            .WithErrorCode("obrigatorio")
            .Length(3, 120)
            .WithErrorCode("tamanho_invalido");

        RuleFor(x => x.Cpf)
            .Must(cpf => ValidadorCpf.EhValido(cpf))
            .WithErrorCode("invalido");

        RuleFor(x => x.DataNascimento)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode("deve_ser_passada");

        RuleFor(x => x.PlanoId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("obrigatorio");
    }
}