using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Desafio.Api.Modules.Planos.Infraestrutura.Repositorios;

public class PlanoRepositorio(AppDbContext db) : IPlanoRepositorio
{
    private const string CodigoViolacaoDeUnicidade = "23505";

    public async Task<IReadOnlyList<Plano>> ListarAsync(CancellationToken cancellationToken) =>
        await db.Planos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);

    public async Task<Plano?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Planos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Plano?> ObterConflitoAsync(
        Guid idExcluir,
        string nome,
        string codigoRegistroAns,
        CancellationToken cancellationToken) =>
        await db.Planos
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(p => p.Id != idExcluir)
            .Where(p => p.Nome == nome || p.CodigoRegistroAns == codigoRegistroAns)
            .FirstOrDefaultAsync(cancellationToken);

    public void Adicionar(Plano plano) => db.Planos.Add(plano);

    // A verificação de unicidade em ObterConflitoAsync não elimina a corrida
    // entre duas requisições simultâneas. A garantia real é o índice único
    // no banco; aqui a violação vira 409.
    public async Task SalvarAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException excecao) when (EhViolacaoDeUnicidade(excecao))
        {
            throw new ConflitoExcecao("Já existe plano cadastrado com esse valor");
        }
    }

    private static bool EhViolacaoDeUnicidade(DbUpdateException excecao) =>
        excecao.InnerException is PostgresException postgres &&
        postgres.SqlState == CodigoViolacaoDeUnicidade;
}
