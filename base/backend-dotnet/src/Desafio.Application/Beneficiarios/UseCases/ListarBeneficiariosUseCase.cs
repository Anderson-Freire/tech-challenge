using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Domain.Beneficiarios;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.UseCases;

public sealed class ListarBeneficiariosUseCase(
    IBeneficiarioRepositorio repositorio,
    IValidator<ListarBeneficiariosRequest> validador)
    : IUseCase<ListarBeneficiariosRequest, ListarBeneficiariosResponse>
{
    public async Task<ListarBeneficiariosResponse> ExecutarAsync(
        ListarBeneficiariosRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        StatusBeneficiario? status = null;

        if (!string.IsNullOrWhiteSpace(entrada.Status))
        {
            status = Enum.Parse<StatusBeneficiario>(entrada.Status, ignoreCase: true);
        }

        var resultado = await repositorio.ListarAsync(
            entrada.Pagina,
            entrada.Tamanho,
            status,
            entrada.PlanoId,
            cancellationToken);

        return new ListarBeneficiariosResponse(
            [.. resultado.Dados.Select(ListarBeneficiariosItemResponse.De)],
            resultado.Pagina,
            resultado.Tamanho,
            resultado.Total);
    }
}
