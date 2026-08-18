using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Dominio.VO;

namespace Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

public partial class Plano
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Nome { get; private set; } = null!;

    public CodigoRegistroAns CodigoRegistroAns { get; private set; } = null!;

    public DateTime? ExcluidoEm { get; private set; } = null;

    private Plano()
    {
    }

    public Plano(string? nome, string? codigoRegistroAns)
        : this(Guid.NewGuid(), nome, codigoRegistroAns)
    {
    }

    public Plano(Guid id, string? nome, string? codigoRegistroAns)
    {
        if (id == Guid.Empty)
        {
            throw new ValidacaoExcecao(
                "Dados do plano inválidos",
                [new DetalheErro("id", "invalido")]);
        }

        Id = id;
        DefinirDados(nome, codigoRegistroAns);
    }

    public bool EstaExcluido => ExcluidoEm is not null;

    public void DefinirDados(string? nome, string? codigoRegistroAns)
    {
        if (EstaExcluido)
        {
            throw new ValidacaoExcecao(
                "Não é possível alterar um plano excluído",
                [new DetalheErro("plano", "excluido")]);
        }

        nome = nome?.Trim() ?? string.Empty;

        var detalhes = new List<DetalheErro>();

        if (nome.Length == 0)
        {
            detalhes.Add(new DetalheErro("nome", "obrigatorio"));
        }
        else if (nome.Length is < 3 or > 60)
        {
            detalhes.Add(new DetalheErro("nome", "tamanho_invalido"));
        }

        if (detalhes.Count > 0)
        {
            throw new ValidacaoExcecao("Dados do plano inválidos", detalhes);
        }

        Nome = nome;
        CodigoRegistroAns = new CodigoRegistroAns(codigoRegistroAns);
    }
    public void Excluir()
    {
        if (EstaExcluido)
        {
            throw new ValidacaoExcecao(
                "Plano já está excluído",
                [new DetalheErro("plano", "ja_excluido")]);
        }

        ExcluidoEm = DateTime.UtcNow;
    }

    public void Restaurar()
    {
        if (!EstaExcluido)
        {
            throw new ValidacaoExcecao(
                "Plano não está excluído",
                [new DetalheErro("plano", "nao_excluido")]);
        }

        ExcluidoEm = null;
    }
}