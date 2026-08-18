using Desafio.Domain.Beneficiarios;
using Desafio.Domain.Planos;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Persistencia;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Plano> Planos => Set<Plano>();

    public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
