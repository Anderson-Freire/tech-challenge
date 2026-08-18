using System.Net;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Extensoes;

namespace Desafio.Api.Api.Comum;

public sealed class Resposta<T>
{
    public HttpStatusCode CodigoStatus { get; }

    public bool Sucesso { get; }

    public string Mensagem { get; }

    public TipoErro? CodigoRestricao { get; }

    public IReadOnlyList<DetalheErro> Detalhes { get; }

    public T? Dados { get; }

    private Resposta(
        HttpStatusCode codigoStatus,
        bool sucesso,
        string mensagem,
        T? dados,
        TipoErro? codigoRestricao,
        IReadOnlyList<DetalheErro>? detalhes)
    {
        CodigoStatus = codigoStatus;
        Sucesso = sucesso;
        Mensagem = mensagem;
        Dados = dados;
        CodigoRestricao = codigoRestricao;
        Detalhes = detalhes ?? [];
    }

    public static Resposta<T> Ok(T dados, HttpStatusCode codigoStatus = HttpStatusCode.OK) =>
        new(codigoStatus, true, "Requisição processada com sucesso", dados, null, null);

    public static Resposta<T> Falha(ExcecaoDeDominio excecao) =>
        new(excecao.ParaStatusCode(), false, excecao.Message, default, excecao.Erro, excecao.Detalhes);

    public static Resposta<T> FalhaInterna() =>
        new(HttpStatusCode.InternalServerError, false, "Erro interno ao processar a requisição", default, null, null);
}
