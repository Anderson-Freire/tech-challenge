using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

public sealed class AtualizarPlanoRequestValidador : AbstractValidator<AtualizarPlanoRequest>
{
    public AtualizarPlanoRequestValidador()
    {
        this.Id(x => x.Id);
        this.Nome(x => x.Nome);
        this.CodigoRegistroAns(x => x.CodigoRegistroAns);
    }
}
