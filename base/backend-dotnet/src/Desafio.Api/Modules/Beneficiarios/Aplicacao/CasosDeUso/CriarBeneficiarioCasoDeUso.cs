using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Mapeamentos;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;

public sealed class CriarBeneficiarioCasoDeUso(
    IBeneficiarioRepositorio beneficiarioRepositorio,
    IPlanoRepositorio planoRepositorio)
    : ICasoDeUso<CriarBeneficiarioRequest, BeneficiarioResponse>
{
    public async Task<BeneficiarioResponse> ExecutarAsync(
        CriarBeneficiarioRequest entrada,
        CancellationToken cancellationToken)
    {
        var plano = await planoRepositorio.ObterPorIdAsync(
            entrada.PlanoId,
            cancellationToken);

        if (plano is null || plano.EstaExcluido)
        {
            throw new NaoProcessavelExcecao(
                "Plano informado não existe",
                [new DetalheErro("plano_id", "inexistente")]);
        }

        var beneficiario = CriarBeneficiarioMapeamento.ParaEntidade(entrada);

        if (await beneficiarioRepositorio.CpfEstaEmUsoAsync(
                beneficiario.Cpf.Valor,
                cancellationToken))
        {
            throw new ConflitoExcecao(
                "CPF já cadastrado",
                [new DetalheErro("cpf", "duplicado")]);
        }

        beneficiarioRepositorio.Adicionar(beneficiario);

        await beneficiarioRepositorio.SalvarAsync(cancellationToken);

        return BeneficiarioMapeamento.ParaResponse(beneficiario);
    }
}