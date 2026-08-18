namespace Desafio.Application.Compartilhado;

public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Dados,
    int Pagina,
    int Tamanho,
    int Total);
