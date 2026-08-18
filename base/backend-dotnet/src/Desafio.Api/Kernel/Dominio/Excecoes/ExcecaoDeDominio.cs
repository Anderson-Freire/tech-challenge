namespace Desafio.Api.Kernel.Dominio.Excecoes;

public abstract class ExcecaoDeDominio : Exception
{
    protected ExcecaoDeDominio(
        TipoErro erro,
        string mensagem,
        IReadOnlyList<DetalheErro>? detalhes,
        Exception? innerException = null)
        : base(mensagem, innerException)
    {
        Erro = erro;
        Detalhes = detalhes ?? [];
    }

    public TipoErro Erro { get; }

    public IReadOnlyList<DetalheErro> Detalhes { get; }
}