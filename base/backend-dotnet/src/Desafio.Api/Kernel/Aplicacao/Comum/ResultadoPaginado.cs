namespace Desafio.Api.Kernel.Aplicacao.Comum;

public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Dados,
    int Pagina,
    int Tamanho,
    int Total);
