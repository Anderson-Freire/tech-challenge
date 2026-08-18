using Desafio.Api.Kernel.Dominio.VO;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Api.Modules.Planos.Infraestrutura.ConfiguracaoBancoDeDados;


public class PlanoConfiguracao : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> construtor)
    {
        construtor.ToTable("plano");

        construtor.HasKey(e => e.Id);

        construtor.Property(e => e.Nome)
            .HasColumnName("nome")
            .HasComment("Nome do Plano")
            .HasColumnType("varchar(60)")
            .IsRequired();

        construtor.Property(e => e.CodigoRegistroAns)
            .HasColumnName("codigo_registro_ans")
            .HasComment("Código de registro do plano na ANS")
            .HasColumnType("varchar(6)")
            .HasConversion(
                codigo => codigo.Valor,
                valor => new CodigoRegistroAns(valor))
            .IsRequired();

        construtor.Property(e => e.ExcluidoEm)
            .HasColumnName("excluido_em")
            .HasComment("Data de exclusao")
            .HasColumnType("timestamp");
    }
}