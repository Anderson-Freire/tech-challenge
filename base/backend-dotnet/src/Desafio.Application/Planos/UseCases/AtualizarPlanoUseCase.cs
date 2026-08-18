using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos.Dtos;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Planos.UseCases;

public sealed class AtualizarPlanoUseCase(
    IPlanoRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<AtualizarPlanoRequest> validador)
    : IUseCase<AtualizarPlanoRequest, AtualizarPlanoResponse>
{
    public async Task<AtualizarPlanoResponse> ExecutarAsync(
        AtualizarPlanoRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var plano = await repositorio
            .ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao("Plano não encontrado");

        plano.Atualizar(entrada.Nome, entrada.CodigoRegistroAns);

        await GarantiaDeUnicidadeDePlano.GarantirAsync(repositorio, plano, cancellationToken);
        await unidadeDeTrabalho.SalvarAsync(cancellationToken);

        return AtualizarPlanoResponse.De(plano);
    }
}
