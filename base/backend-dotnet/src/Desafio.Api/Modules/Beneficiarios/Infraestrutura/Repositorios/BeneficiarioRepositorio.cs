using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Desafio.Api.Modules.Beneficiarios.Infraestrutura.Repositorios;

public class BeneficiarioRepositorio(AppDbContext db) : IBeneficiarioRepositorio
{
    private const string CodigoViolacaoDeUnicidade = "23505";

    public async Task<Beneficiario?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        await db.Beneficiarios
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<bool> CpfEstaEmUsoAsync(
        string cpf,
        CancellationToken cancellationToken) =>
        await db.Beneficiarios
            .IgnoreQueryFilters()
            .AnyAsync(b => b.Cpf.Valor == cpf, cancellationToken);

    public async Task<ResultadoPaginado<Beneficiario>> ListarAsync(
        int pagina,
        int tamanho,
        StatusBeneficiario? status,
        Guid? planoId,
        CancellationToken cancellationToken)
    {
        var query = db.Beneficiarios
            .AsNoTracking()
            .AsQueryable();

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
            .OrderBy(b => b.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Beneficiario>(
            dados,
            pagina,
            tamanho,
            total);
    }

    public void Adicionar(Beneficiario beneficiario) =>
        db.Beneficiarios.Add(beneficiario);

    public async Task SalvarAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException excecao)
            when (EhViolacaoDeUnicidade(excecao))
        {
            throw new ConflitoExcecao(
                "CPF já cadastrado",
                [new DetalheErro("cpf", "duplicado")]);
        }
    }

    private static bool EhViolacaoDeUnicidade(
        DbUpdateException excecao) =>
        excecao.InnerException is PostgresException postgres &&
        postgres.SqlState == CodigoViolacaoDeUnicidade;
}