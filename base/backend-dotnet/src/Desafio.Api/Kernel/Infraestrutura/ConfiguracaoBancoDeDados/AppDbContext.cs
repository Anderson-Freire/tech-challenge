using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;
using Desafio.Api.Modules.Planos.Infraestrutura.ConfiguracaoBancoDeDados;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Plano> Planos => Set<Plano>();
    // public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new PlanoConfiguracao());
        // base.OnModelCreating(modelBuilder);

        // modelBuilder.ApplyConfigurationsFromAssembly(
        //     typeof(AppDbContext).Assembly);
    }
}