using Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Api.Controllers;

[ApiController]
[Route("health")]
[Produces("application/json")]
public sealed class HealthController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Obter(
        CancellationToken cancellationToken)
    {
        var bancoDisponivel = await db.Database.CanConnectAsync(
            cancellationToken);

        var resposta = new
        {
            Status = bancoDisponivel ? "ok" : "indisponivel",
            Banco = bancoDisponivel ? "ok" : "indisponivel"
        };

        return bancoDisponivel
            ? Ok(resposta)
            : StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                resposta);
    }
}