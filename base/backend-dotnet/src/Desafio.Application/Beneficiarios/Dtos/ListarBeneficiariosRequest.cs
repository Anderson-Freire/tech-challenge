namespace Desafio.Application.Beneficiarios.Dtos;

public sealed record ListarBeneficiariosRequest(
    int Pagina = 1,
    int Tamanho = 10,
    string? Status = null,
    Guid? PlanoId = null);
