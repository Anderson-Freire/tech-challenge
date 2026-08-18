namespace Desafio.Domain.Compartilhado.Excecoes;

public abstract class ExcecaoDeDominio(
    TipoErro erro,
    string mensagem,
    IReadOnlyList<DetalheErro>? detalhes,
    Exception? innerException = null)
    : Exception(mensagem, innerException)
{
    public TipoErro Erro { get; } = erro;

    public IReadOnlyList<DetalheErro> Detalhes { get; } = detalhes ?? [];
}
