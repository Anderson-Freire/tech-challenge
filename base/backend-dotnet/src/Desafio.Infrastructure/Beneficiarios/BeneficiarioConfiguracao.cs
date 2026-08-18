using Desafio.Domain.Beneficiarios;
using Desafio.Domain.Compartilhado.ObjetosDeValor;
using Desafio.Domain.Planos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Infrastructure.Beneficiarios;

public sealed class BeneficiarioConfiguracao : IEntityTypeConfiguration<Beneficiario>
{
    public void Configure(EntityTypeBuilder<Beneficiario> construtor)
    {
        construtor.ToTable("Beneficiarios");

        construtor.HasKey(b => b.Id);

        construtor.Property(b => b.NomeCompleto)
            .HasMaxLength(120)
            .IsRequired();

        construtor.Property(b => b.Cpf)
            .HasMaxLength(11)
            .HasConversion(
                cpf => cpf.Valor,
                valor => new Cpf(valor))
            .IsRequired();

        construtor.Property(b => b.DataNascimento)
            .IsRequired();

        construtor.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        construtor.Property(b => b.DataCadastro)
            .IsRequired();

        construtor.Property(b => b.ExcluidoEm);

        construtor.HasIndex(b => b.Cpf).IsUnique();

        construtor.HasOne<Plano>()
            .WithMany()
            .HasForeignKey(b => b.PlanoId)
            .OnDelete(DeleteBehavior.Restrict);

        construtor.HasQueryFilter(b => b.ExcluidoEm == null);
    }
}
