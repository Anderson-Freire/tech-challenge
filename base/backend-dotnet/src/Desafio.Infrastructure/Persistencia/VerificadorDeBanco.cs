using Desafio.Application.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Persistencia;

public sealed class VerificadorDeBanco(AppDbContext db) : IVerificadorDeBanco
{
    public Task<bool> EstaDisponivelAsync(CancellationToken cancellationToken) =>
        db.Database.CanConnectAsync(cancellationToken);
}
