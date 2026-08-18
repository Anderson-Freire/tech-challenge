namespace Desafio.Application.Compartilhado;

public interface IUnidadeDeTrabalho
{
    Task SalvarAsync(CancellationToken cancellationToken);
}
