namespace Desafio.Api.Kernel.Dominio.Excecoes;

public sealed class ConflitoExcecao(
    string mensagem,
    IReadOnlyList<DetalheErro>? detalhes = null,
    Exception? innerException = null)
    : ExcecaoDeDominio(TipoErro.Conflito, mensagem, detalhes, innerException);