using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Domain.Compartilhado.ObjetosDeValor;

namespace Desafio.Domain.Planos;

public sealed class Plano()
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Nome { get; private set; } = null!;

    public CodigoRegistroAns CodigoRegistroAns { get; private set; } = null!;

    public DateTime? ExcluidoEm { get; private set; }

    public bool EstaExcluido => ExcluidoEm is not null;

    public Plano(string? nome, string? codigoRegistroAns)
        : this(Guid.NewGuid(), nome, codigoRegistroAns)
    {
    }

    public Plano(Guid id, string? nome, string? codigoRegistroAns) : this()
    {
        if (id == Guid.Empty)
        {
            throw new ValidacaoExcecao(
                "Dados do plano inválidos",
                [new DetalheErro(nameof(Id), "invalido")]);
        }

        Id = id;
        Atualizar(nome, codigoRegistroAns);
    }

    public void Atualizar(string? nome, string? codigoRegistroAns)
    {
        if (EstaExcluido)
        {
            throw new NaoEncontradoExcecao("Plano não encontrado");
        }

        nome = nome?.Trim() ?? string.Empty;

        var detalhes = new List<DetalheErro>();

        if (nome.Length == 0)
        {
            detalhes.Add(new DetalheErro(nameof(Nome), "obrigatorio"));
        }
        else if (nome.Length is < LimitesDePlano.NomeMinimo or > LimitesDePlano.NomeMaximo)
        {
            detalhes.Add(new DetalheErro(nameof(Nome), "tamanho_invalido"));
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
            throw new NaoEncontradoExcecao("Plano não encontrado");
        }

        ExcluidoEm = DateTime.UtcNow;
    }
}
