using Desafio.Api.Api.Comum;
using Desafio.Api.Api.Serializacao;
using Desafio.Api.Kernel.Dominio.Excecoes;

namespace Desafio.Api.Api.Middlewares;

public class TratamentoDeExcecaoMiddleware(RequestDelegate proximo, ILogger<TratamentoDeExcecaoMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await proximo(contexto);
        }
        catch (ExcecaoDeDominio excecao)
        {
            logger.LogWarning(
                "Requisição recusada. Rota: {Rota}. Erro: {Erro}. Mensagem: {Mensagem}",
                contexto.Request.Path,
                excecao.Erro,
                excecao.Message);

            var resposta = Resposta<object?>.Falha(excecao);
            await EscreverAsync(contexto, resposta);
        }
        catch (Exception excecao)
        {
            logger.LogError(
                excecao,
                "Falha não tratada. Rota: {Rota}. Método: {Metodo}",
                contexto.Request.Path,
                contexto.Request.Method);

            var resposta = Resposta<object?>.FalhaInterna();
            await EscreverAsync(contexto, resposta);
        }
    }

    private static async Task EscreverAsync(HttpContext contexto, Resposta<object?> resposta)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        contexto.Response.Clear();
        contexto.Response.StatusCode = (int)resposta.CodigoStatus;
        contexto.Response.ContentType = "application/json; charset=utf-8";

        var corpo = System.Text.Json.JsonSerializer.Serialize(resposta, JsonPadrao.Opcoes);
        await contexto.Response.WriteAsync(corpo);
    }
}
