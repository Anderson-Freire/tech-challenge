namespace Desafio.Api.Kernel.Dominio.Excecoes;

public sealed class ValidacaoExcecao(
    string mensagem,
    IReadOnlyList<DetalheErro>? detalhes = null,
    Exception? innerException = null)
    : ExcecaoDeDominio(TipoErro.ValidacaoInvalida, mensagem, detalhes, innerException);