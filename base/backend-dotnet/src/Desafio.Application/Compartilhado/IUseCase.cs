namespace Desafio.Application.Compartilhado;

public interface IUseCase<in TEntrada, TSaida>
{
    Task<TSaida> ExecutarAsync(TEntrada entrada, CancellationToken cancellationToken);
}

public interface IUseCaseSemSaida<in TEntrada>
{
    Task ExecutarAsync(TEntrada entrada, CancellationToken cancellationToken);
}
