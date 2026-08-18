namespace Desafio.Api.Middlewares;

public sealed class LogDeRequisicaoMiddleware(
    RequestDelegate proximo,
    ILogger<LogDeRequisicaoMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext contexto)
    {
        await proximo(contexto);

        logger.LogInformation(
            "Requisição. Metodo: {Metodo}. Rota: {Rota}. Status: {Status}",
            contexto.Request.Method,
            contexto.Request.Path,
            contexto.Response.StatusCode);
    }
}
