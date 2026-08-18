namespace Desafio.Domain.Compartilhado.Excecoes;

public sealed class NaoEncontradoExcecao(
    string mensagem,
    Exception? innerException = null)
    : ExcecaoDeDominio(TipoErro.NaoEncontrado, mensagem, null, innerException);