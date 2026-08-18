namespace Desafio.Domain.Compartilhado.Excecoes;

public sealed class NaoProcessavelExcecao(
    string mensagem,
    IReadOnlyList<DetalheErro>? detalhes = null,
    Exception? innerException = null)
    : ExcecaoDeDominio(TipoErro.NaoProcessavel, mensagem, detalhes, innerException);