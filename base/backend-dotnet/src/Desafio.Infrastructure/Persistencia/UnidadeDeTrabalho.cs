using Desafio.Application.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Persistencia;

public sealed class UnidadeDeTrabalho(AppDbContext db) : IUnidadeDeTrabalho
{
    public async Task SalvarAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException excecao) when (ViolacaoDeUnicidade.Eh(excecao))
        {
            throw ViolacaoDeUnicidade.ParaExcecao(excecao);
        }
    }
}
