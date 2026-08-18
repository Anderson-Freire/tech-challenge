using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;

// Usado apenas pelas ferramentas de linha de comando do EF Core,
// para criar o DbContext durante a geração das migrations.
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conexao = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");

        if (string.IsNullOrWhiteSpace(conexao))
        {
            throw new InvalidOperationException(
                "A variável de ambiente 'ConnectionStrings__Postgres' não foi definida.");
        }

        var opcoes = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conexao)
            .Options;

        return new AppDbContext(opcoes);
    }
}