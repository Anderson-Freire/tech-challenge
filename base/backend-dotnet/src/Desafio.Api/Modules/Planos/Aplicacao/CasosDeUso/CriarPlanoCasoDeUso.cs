using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;

public sealed class CriarPlanoCasoDeUso(
    IPlanoRepositorio repositorio)
    : ICasoDeUso<CriarPlanoRequest, PlanoResponse>
{
    public async Task<PlanoResponse> ExecutarAsync(
        CriarPlanoRequest entrada,
        CancellationToken cancellationToken)
    {
        var plano = new Plano(
            entrada.Nome,
            entrada.CodigoRegistroAns);

        await GarantirUnicidadeAsync(
            plano,
            cancellationToken);

        repositorio.Adicionar(plano);

        await repositorio.SalvarAsync(cancellationToken);

        return PlanoMapeamento.ParaResponse(plano);
    }

    private async Task GarantirUnicidadeAsync(
        Plano plano,
        CancellationToken cancellationToken)
    {
        var conflito = await repositorio.ObterConflitoAsync(
            plano.Id,
            plano.Nome,
            plano.CodigoRegistroAns.Valor,
            cancellationToken);

        if (conflito is null)
        {
            return;
        }

        var campo = conflito.Nome == plano.Nome
            ? "nome"
            : "codigo_registro_ans";

        throw new ConflitoExcecao(
            "Já existe plano cadastrado com esse valor",
            [new DetalheErro(campo, "duplicado")]);
    }
}