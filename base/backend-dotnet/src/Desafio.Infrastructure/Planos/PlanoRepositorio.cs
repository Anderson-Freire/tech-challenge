using Desafio.Application.Planos;
using Desafio.Domain.Planos;
using Desafio.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Planos;

public sealed class PlanoRepositorio(AppDbContext db) : IPlanoRepositorio
{
    public async Task<IReadOnlyList<Plano>> ListarAsync(CancellationToken cancellationToken) =>
        await db.Planos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);

    public Task<Plano?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Planos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Plano?> ObterConflitoAsync(
        Guid idAtual,
        string nome,
        string codigoRegistroAns,
        CancellationToken cancellationToken) =>
        db.Planos
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(p => p.Id != idAtual)
            .Where(p =>
                p.Nome == nome ||
                EF.Property<string>(p, nameof(Plano.CodigoRegistroAns)) == codigoRegistroAns)
            .FirstOrDefaultAsync(cancellationToken);

    public void Adicionar(Plano plano) => db.Planos.Add(plano);
}
