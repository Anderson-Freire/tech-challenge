using Desafio.Api.Kernel.Dominio.VO;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Api.Modules.Beneficiarios.Infraestrutura.ConfiguracaoBancoDeDados;

public class BeneficiarioConfiguracao : IEntityTypeConfiguration<Beneficiario>
{
    public void Configure(EntityTypeBuilder<Beneficiario> construtor)
    {
        construtor.ToTable("beneficiarios");

        construtor.HasKey(e => e.Id);

        construtor.Property(e => e.PlanoId)
            .HasColumnName("plano_id")
            .IsRequired();

        construtor.Property(e => e.NomeCompleto)
            .HasColumnName("nome_completo")
            .HasComment("Nome completo do Beneficiario")
            .HasColumnType("varchar(255)")
            .IsRequired();

        construtor.Property(e => e.Cpf)
            .HasColumnName("cpf")
            .HasComment("CPF do Beneficiario")
            .HasColumnType("varchar(11)")
            .HasConversion(
                cpf => cpf.Valor,
                valor => new Cpf(valor))
            .IsRequired();

        construtor.Property(e => e.DataNascimento)
            .HasColumnName("data_nascimento")
            .HasComment("Data de nascimento do Beneficiario")
            .HasColumnType("date")
            .IsRequired();

        construtor.Property(e => e.Status)
            .HasColumnName("status")
            .HasComment("Status do Beneficiario")
            .HasColumnType("varchar(50)")
            .HasConversion<string>()
            .IsRequired();

        construtor.Property(e => e.DataCadastro)
            .HasColumnName("data_cadastro")
            .HasComment("Data de cadastro")
            .HasColumnType("timestamp")
            .IsRequired();

        construtor.Property(e => e.ExcluidoEm)
            .HasColumnName("excluido_em")
            .HasComment("Data de exclusao")
            .HasColumnType("timestamp");

        construtor.HasOne(e => e.Plano)
            .WithMany()
            .HasForeignKey(e => e.PlanoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}