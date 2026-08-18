using Desafio.Application.Planos.Dtos;
using FluentValidation;

namespace Desafio.Application.Planos.Validadores;

public sealed class ListarPlanosRequestValidador : AbstractValidator<ListarPlanosRequest>
{
    public ListarPlanosRequestValidador()
    {
    }
}
