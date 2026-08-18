using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

public sealed class CriarPlanoRequestValidador : AbstractValidator<CriarPlanoRequest>
{
    public CriarPlanoRequestValidador()
    {
        this.Nome(x => x.Nome);
        this.CodigoRegistroAns(x => x.CodigoRegistroAns);
    }
}
