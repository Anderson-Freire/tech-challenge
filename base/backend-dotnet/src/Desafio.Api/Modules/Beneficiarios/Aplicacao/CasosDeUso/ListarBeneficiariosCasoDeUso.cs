using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;

public class ListarBeneficiariosCasoDeUso(
    IBeneficiarioRepositorio repositorio)
    : ICasoDeUso<
        FiltroListarBeneficiariosRequest,
        ResultadoPaginado<BeneficiarioResponse>>
{
    public async Task<ResultadoPaginado<BeneficiarioResponse>> ExecutarAsync(
        FiltroListarBeneficiariosRequest entrada,
        CancellationToken cancellationToken)
    {
        StatusBeneficiario? status = ConverterStatus(entrada.Status);

        var resultado = await repositorio.ListarAsync(
            entrada.Pagina,
            entrada.Tamanho,
            status,
            entrada.PlanoId,
            cancellationToken);

        return new ResultadoPaginado<BeneficiarioResponse>(
            [.. resultado.Dados.Select(BeneficiarioMapeamento.ParaResponse)],
            resultado.Pagina,
            resultado.Tamanho,
            resultado.Total);
    }

    private static StatusBeneficiario? ConverterStatus(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        if (!Enum.TryParse<StatusBeneficiario>(
                valor,
                ignoreCase: true,
                out var status))
        {
            throw new ValidacaoExcecao(
                "Status inválido",
                [new DetalheErro("status", "invalido")]);
        }

        return status;
    }
}