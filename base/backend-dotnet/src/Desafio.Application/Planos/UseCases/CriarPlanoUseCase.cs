using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using Desafio.Domain.Planos;
using FluentValidation;

namespace Desafio.Application.Planos.UseCases;

public sealed class CriarPlanoUseCase(
    IPlanoRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarPlanoRequest> validador)
    : IUseCase<CriarPlanoRequest, CriarPlanoResponse>
{
    public async Task<CriarPlanoResponse> ExecutarAsync(
        CriarPlanoRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var plano = new Plano(entrada.Nome, entrada.CodigoRegistroAns);

        await GarantiaDeUnicidadeDePlano.GarantirAsync(repositorio, plano, cancellationToken);

        repositorio.Adicionar(plano);
        await unidadeDeTrabalho.SalvarAsync(cancellationToken);

        return CriarPlanoResponse.De(plano);
    }
}
