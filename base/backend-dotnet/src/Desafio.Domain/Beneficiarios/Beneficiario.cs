using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Domain.Compartilhado.ObjetosDeValor;

namespace Desafio.Domain.Beneficiarios;

public sealed class Beneficiario()
{
    public Guid Id { get; private set; }

    public string NomeCompleto { get; private set; } = null!;

    public Cpf Cpf { get; private set; } = null!;

    public DateOnly DataNascimento { get; private set; }

    public StatusBeneficiario Status { get; private set; }

    public DateTime DataCadastro { get; private set; }

    public DateTime? ExcluidoEm { get; private set; }

    public Guid PlanoId { get; private set; }

    public bool EstaExcluido => ExcluidoEm is not null;

    public Beneficiario(
        string? nomeCompleto,
        string? cpf,
        DateOnly dataNascimento,
        Guid planoId) : this()
    {
        Id = Guid.NewGuid();
        DataCadastro = DateTime.UtcNow;
        Status = StatusBeneficiario.ATIVO;
        Cpf = new Cpf(cpf);
        DefinirDadosCadastrais(nomeCompleto, dataNascimento, planoId);
    }

    public void Atualizar(
        string? nomeCompleto,
        DateOnly dataNascimento,
        Guid planoId,
        StatusBeneficiario status)
    {
        if (Status == StatusBeneficiario.INATIVO)
        {
            RecusarAlteracaoCadastralSeCongelado(nomeCompleto, dataNascimento, planoId);
        }

        DefinirDadosCadastrais(nomeCompleto, dataNascimento, planoId);
        Status = status;
    }

    public void Excluir()
    {
        if (EstaExcluido)
        {
            throw new NaoEncontradoExcecao("Beneficiário não encontrado");
        }

        ExcluidoEm = DateTime.UtcNow;
    }

    private void RecusarAlteracaoCadastralSeCongelado(
        string? nomeCompleto,
        DateOnly dataNascimento,
        Guid planoId)
    {
        var nome = (nomeCompleto ?? string.Empty).Trim();
        var alterouCadastro =
            !string.Equals(NomeCompleto, nome, StringComparison.Ordinal) ||
            DataNascimento != dataNascimento ||
            PlanoId != planoId;

        if (!alterouCadastro)
        {
            return;
        }

        throw new ConflitoExcecao(
            "Beneficiário inativo não pode ter dados cadastrais alterados",
            [new DetalheErro(nameof(Status), "inativo")]);
    }

    private void DefinirDadosCadastrais(
        string? nomeCompleto,
        DateOnly dataNascimento,
        Guid planoId)
    {
        var nome = nomeCompleto?.Trim() ?? string.Empty;
        var detalhes = new List<DetalheErro>();

        if (nome.Length == 0)
        {
            detalhes.Add(new DetalheErro(nameof(NomeCompleto), "obrigatorio"));
        }
        else if (nome.Length is < LimitesDeBeneficiario.NomeMinimo or > LimitesDeBeneficiario.NomeMaximo)
        {
            detalhes.Add(new DetalheErro(nameof(NomeCompleto), "tamanho_invalido"));
        }

        if (dataNascimento == default || dataNascimento >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            detalhes.Add(new DetalheErro(nameof(DataNascimento), "deve_ser_passada"));
        }

        if (planoId == Guid.Empty)
        {
            detalhes.Add(new DetalheErro(nameof(PlanoId), "obrigatorio"));
        }

        if (detalhes.Count > 0)
        {
            throw new ValidacaoExcecao("Dados do beneficiário inválidos", detalhes);
        }

        NomeCompleto = nome;
        DataNascimento = dataNascimento;
        PlanoId = planoId;
    }
}
