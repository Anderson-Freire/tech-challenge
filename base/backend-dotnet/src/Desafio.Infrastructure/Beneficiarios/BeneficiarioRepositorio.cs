using Desafio.Application.Beneficiarios;
using Desafio.Application.Compartilhado;
using Desafio.Domain.Beneficiarios;
using Desafio.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Beneficiarios;

public sealed class BeneficiarioRepositorio(AppDbContext db) : IBeneficiarioRepositorio
{
    public Task<Beneficiario?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        db.Beneficiarios.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<bool> CpfEstaEmUsoAsync(
        string cpf,
        CancellationToken cancellationToken) =>
        db.Beneficiarios
            .IgnoreQueryFilters()
            .AnyAsync(
                b => EF.Property<string>(b, nameof(Beneficiario.Cpf)) == cpf,
                cancellationToken);

    public async Task<ResultadoPaginado<Beneficiario>> ListarAsync(
        int pagina,
        int tamanho,
        StatusBeneficiario? status,
        Guid? planoId,
        CancellationToken cancellationToken)
    {
        var query = db.Beneficiarios.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (planoId.HasValue)
        {
            query = query.Where(b => b.PlanoId == planoId.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var dados = await query
            .OrderBy(b => b.DataCadastro)
            .ThenBy(b => b.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Beneficiario>(dados, pagina, tamanho, total);
    }

    public void Adicionar(Beneficiario beneficiario) =>
        db.Beneficiarios.Add(beneficiario);
}
