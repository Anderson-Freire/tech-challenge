using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.Interfaces;

public interface IPlanoRepositorio
{
    Task<IReadOnlyList<Plano>> ListarAsync(CancellationToken cancellationToken);

    Task<Plano?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    // Considera até planos excluídos, pois nome e código ANS continuam ocupados
    // depois da exclusão lógica.
    Task<Plano?> ObterConflitoAsync(
        Guid idExcluir,
        string nome,
        string codigoRegistroAns,
        CancellationToken cancellationToken);

    void Adicionar(Plano plano);

    Task SalvarAsync(CancellationToken cancellationToken);
}