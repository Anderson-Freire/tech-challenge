using Desafio.Domain.Compartilhado.ObjetosDeValor;
using Desafio.Domain.Planos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Infrastructure.Planos;

public sealed class PlanoConfiguracao : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> construtor)
    {
        construtor.ToTable("Planos");

        construtor.HasKey(p => p.Id);

        construtor.Property(p => p.Nome)
            .HasMaxLength(60)
            .IsRequired();

        construtor.Property(p => p.CodigoRegistroAns)
            .HasMaxLength(6)
            .HasConversion(
                codigo => codigo.Valor,
                valor => new CodigoRegistroAns(valor))
            .IsRequired();

        construtor.Property(p => p.ExcluidoEm);

        construtor.HasIndex(p => p.Nome).IsUnique();
        construtor.HasIndex(p => p.CodigoRegistroAns).IsUnique();

        construtor.HasQueryFilter(p => p.ExcluidoEm == null);
    }
}
