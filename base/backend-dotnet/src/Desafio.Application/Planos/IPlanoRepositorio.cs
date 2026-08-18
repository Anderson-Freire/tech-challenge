using Desafio.Domain.Planos;

namespace Desafio.Application.Planos;

public interface IPlanoRepositorio
{
    Task<IReadOnlyList<Plano>> ListarAsync(CancellationToken cancellationToken);

    Task<Plano?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Plano?> ObterConflitoAsync(
        Guid idAtual,
        string nome,
        string codigoRegistroAns,
        CancellationToken cancellationToken);

    void Adicionar(Plano plano);
}
