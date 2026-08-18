using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;

public sealed record AtualizarBeneficiarioEntrada(
    Guid Id,
    AtualizarBeneficiarioRequest Dados);

public sealed class AtualizarBeneficiarioCasoDeUso(
    IBeneficiarioRepositorio beneficiarioRepositorio,
    IPlanoRepositorio planoRepositorio)
    : ICasoDeUso<AtualizarBeneficiarioEntrada, BeneficiarioResponse>
{
    public async Task<BeneficiarioResponse> ExecutarAsync(
        AtualizarBeneficiarioEntrada entrada,
        CancellationToken cancellationToken)
    {
        var beneficiario = await beneficiarioRepositorio
            .ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Beneficiário não encontrado");

        var dados = entrada.Dados;

        var alterouPlano = beneficiario.PlanoId != dados.PlanoId;

        var precisaValidarPlano =
            alterouPlano ||
            dados.Status == StatusBeneficiario.ATIVO;

        if (precisaValidarPlano)
        {
            var plano = await planoRepositorio.ObterPorIdAsync(
                dados.PlanoId,
                cancellationToken);

            if (plano is null || plano.EstaExcluido)
            {
                throw new NaoProcessavelExcecao(
                    "Plano informado não existe",
                    [new DetalheErro("plano_id", "inexistente")]);
            }
        }

        AtualizarBeneficiarioMapeamento.Aplicar(
            beneficiario,
            dados);

        await beneficiarioRepositorio.SalvarAsync(cancellationToken);

        return BeneficiarioMapeamento.ParaResponse(beneficiario);
    }
}