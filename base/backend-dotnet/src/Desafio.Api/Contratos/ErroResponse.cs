using System.Text.Json;
using Desafio.Domain.Compartilhado.Excecoes;

namespace Desafio.Api.Contratos;

public sealed record ErroResponse(
    string Erro,
    string Mensagem,
    IReadOnlyList<DetalheErro> Detalhes)
{
    public static ErroResponse De(ExcecaoDeDominio excecao) =>
        new(excecao.Erro.ToString(), excecao.Message, ParaContrato(excecao.Detalhes));

    public static ErroResponse Validacao(string mensagem, IEnumerable<DetalheErro> detalhes) =>
        new(nameof(TipoErro.ValidacaoInvalida), mensagem, ParaContrato(detalhes));

    private static IReadOnlyList<DetalheErro> ParaContrato(IEnumerable<DetalheErro> detalhes) =>
        [.. detalhes.Select(detalhe => detalhe with
        {
            Campo = JsonNamingPolicy.SnakeCaseLower.ConvertName(detalhe.Campo)
        })];
}
