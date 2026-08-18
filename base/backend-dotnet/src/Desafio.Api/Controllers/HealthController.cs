using Desafio.Application.Compartilhado;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Controllers;

[ApiController]
[Route("health")]
[Produces("application/json")]
public sealed class HealthController(IVerificadorDeBanco banco) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObterAsync(CancellationToken cancellationToken)
    {
        var bancoDisponivel = await banco.EstaDisponivelAsync(cancellationToken);

        var resposta = new
        {
            Status = bancoDisponivel ? "ok" : "indisponivel",
            Banco = bancoDisponivel ? "ok" : "indisponivel"
        };

        return bancoDisponivel
            ? Ok(resposta)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, resposta);
    }
}
