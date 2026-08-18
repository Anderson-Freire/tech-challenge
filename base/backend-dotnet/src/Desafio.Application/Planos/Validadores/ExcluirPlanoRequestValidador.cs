using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

public sealed class ExcluirPlanoRequestValidador : AbstractValidator<ExcluirPlanoRequest>
{
    public ExcluirPlanoRequestValidador()
    {
        this.Id(x => x.Id);
    }
}
