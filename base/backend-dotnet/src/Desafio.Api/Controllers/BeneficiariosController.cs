using Desafio.Api.Dominio;
using Desafio.Api.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Api.Controllers;

[ApiController]
[Route("beneficiarios")]
[Produces("application/json")]
public class BeneficiariosController : ControllerBase
{
    private readonly AppDbContext _db;

    public BeneficiariosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Beneficiario beneficiario)
    {
        if (beneficiario.Cpf.Length == 11)
        {
            var existe = await _db.Beneficiarios
                .IgnoreQueryFilters()
                .AnyAsync(b => b.Cpf == beneficiario.Cpf);

            if (existe)
            {
                return Conflict("CPF ja cadastrado");
            }

            var planoExiste = await _db.Planos
                .AnyAsync(p => p.Id == beneficiario.PlanoId);

            if (!planoExiste)
            {
                throw new NaoProcessavelException(
                    "Plano informado não existe",
                    [new DetalheErro("plano_id", "inexistente")]);
            }

            _db.Beneficiarios.Add(beneficiario);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Obter),
                new { id = beneficiario.Id },
                beneficiario);
        }

        return BadRequest("CPF invalido");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var beneficiario = await _db.Beneficiarios
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario is null)
        {
            return NotFound();
        }

        return Ok(beneficiario);
    }

    public sealed class AtualizarBeneficiarioRequest
    {
        public string NomeCompleto { get; set; } = null!;

        public DateOnly DataNascimento { get; set; }

        public Guid PlanoId { get; set; }

        public StatusBeneficiario Status { get; set; }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
    Guid id,
    [FromBody] AtualizarBeneficiarioRequest dados)
    {
        var beneficiario = await _db.Beneficiarios
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario is null)
        {
            return NotFound();
        }

        var planoExiste = await _db.Planos
            .AnyAsync(p => p.Id == dados.PlanoId);

        if (!planoExiste)
        {
            throw new NaoProcessavelException(
                "Plano informado não existe",
                [new DetalheErro("plano_id", "inexistente")]);
        }

        beneficiario.NomeCompleto = dados.NomeCompleto;
        beneficiario.DataNascimento = dados.DataNascimento;
        beneficiario.PlanoId = dados.PlanoId;
        beneficiario.Status = dados.Status;

        await _db.SaveChangesAsync();

        return Ok(beneficiario);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20,
        [FromQuery] string? status = null,
        [FromQuery(Name = "plano_id")] Guid? planoId = null)
    {
        if (pagina < 1)
        {
            return BadRequest("A página deve ser maior ou igual a 1.");
        }

        if (tamanho < 1)
        {
            return BadRequest("A tamanho deve ser maior ou igual a 1.");
        }

        var query = _db.Beneficiarios
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusBeneficiario>(
                    status,
                    ignoreCase: true,
                    out var statusEnum))
            {
                return BadRequest("Status inválido.");
            }

            query = query.Where(b => b.Status == statusEnum);
        }

        if (planoId.HasValue)
        {
            query = query.Where(b => b.PlanoId == planoId.Value);
        }

        var total = await query.CountAsync();

        var lista = await query
            .OrderBy(b => b.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(b => new
            {
                b.Id,
                b.NomeCompleto,
                b.Cpf,
                b.DataNascimento,
                b.Status,
                b.PlanoId,
                b.DataCadastro,
                Plano = b.Plano == null
                    ? null
                    : new
                    {
                        b.Plano.Id,
                        b.Plano.Nome,
                        b.Plano.CodigoRegistroAns
                    }
            })
            .ToListAsync();

        return Ok(new
        {
            dados = lista,
            pagina,
            tamanho,
            total
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var beneficiario = await _db.Beneficiarios
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario is null)
        {
            return NotFound();
        }

        _db.Entry(beneficiario)
            .Property("ExcluidoEm")
            .CurrentValue = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return NoContent();
    }
}

