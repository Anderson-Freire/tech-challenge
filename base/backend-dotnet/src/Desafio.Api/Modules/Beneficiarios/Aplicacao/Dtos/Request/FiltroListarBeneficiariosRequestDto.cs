namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;

public sealed record FiltroListarBeneficiariosRequest(
    int Pagina = 1,
    int Tamanho = 20,
    string? Status = null,
    Guid? PlanoId = null);