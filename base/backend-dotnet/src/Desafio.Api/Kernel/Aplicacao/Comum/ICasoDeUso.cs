namespace Desafio.Api.Kernel.Aplicacao.Comum;

public interface ICasoDeUso<in TEntrada, TSaida>
{
    Task<TSaida> ExecutarAsync(TEntrada entrada, CancellationToken cancellationToken);
}

public interface ICasoDeUso<TSaida>
{
    Task<TSaida> ExecutarAsync(CancellationToken cancellationToken);
}
