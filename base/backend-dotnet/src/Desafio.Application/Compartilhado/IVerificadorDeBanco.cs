namespace Desafio.Application.Compartilhado;

public interface IVerificadorDeBanco
{
    Task<bool> EstaDisponivelAsync(CancellationToken cancellationToken);
}
