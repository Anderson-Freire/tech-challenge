using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Dominio.VO;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;

public class Beneficiario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string NomeCompleto { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public DateOnly DataNascimento { get; private set; }
    public StatusBeneficiario Status { get; private set; } = StatusBeneficiario.ATIVO;
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
    public DateTime? ExcluidoEm { get; private set; } = null;
    public Guid PlanoId { get; private set; }
    public Plano? Plano { get; private set; }

    private Beneficiario()
    {
    }

    public Beneficiario(string nomeCompleto, string cpf, DateOnly dataNascimento, Guid planoId)
    {
        var detalhes = new List<DetalheErro>();

        nomeCompleto = nomeCompleto?.Trim() ?? string.Empty;
        if (nomeCompleto.Length == 0)
        {
            detalhes.Add(new DetalheErro("nome_completo", "obrigatorio"));
        }

        if (detalhes.Count > 0)
        {
            throw new ValidacaoExcecao("Dados do beneficiário inválidos", detalhes);
        }

        NomeCompleto = nomeCompleto;
        Cpf = new Cpf(cpf); // validação do CPF acontece aqui dentro do VO
        DataNascimento = dataNascimento;
        PlanoId = planoId;
    }

    public void Atualizar(
        string nomeCompleto,
        DateOnly dataNascimento,
        Guid planoId,
        StatusBeneficiario status)
    {
        NomeCompleto = nomeCompleto;
        DataNascimento = dataNascimento;
        PlanoId = planoId;
        Status = status;
    }

    public void AlterarStatus(StatusBeneficiario status)
    {
        Status = status;
    }

    public bool EstaExcluido => ExcluidoEm is not null;

    public void Excluir()
    {
        if (EstaExcluido)
        {
            throw new ValidacaoExcecao(
                "Beneficiário já está excluído",
                [new DetalheErro("beneficiario", "ja_excluido")]);
        }

        ExcluidoEm = DateTime.UtcNow;
    }
}