using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

public sealed class ObterPlanoRequestValidador : AbstractValidator<ObterPlanoRequest>
{
    public ObterPlanoRequestValidador()
    {
        this.Id(x => x.Id);
    }
}
