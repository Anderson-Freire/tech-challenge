using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Response;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;

public sealed record AtualizarPlanoEntrada(
    Guid Id,
    AtualizarPlanoRequest Dados);

public sealed class AtualizarPlanoCasoDeUso(
    IPlanoRepositorio repositorio)
    : ICasoDeUso<AtualizarPlanoEntrada, PlanoResponse>
{
    public async Task<PlanoResponse> ExecutarAsync(
        AtualizarPlanoEntrada entrada,
        CancellationToken cancellationToken)
    {
        var plano = await repositorio
            .ObterPorIdAsync(entrada.Id, cancellationToken)
            ?? throw new NaoEncontradoExcecao(
                "Plano não encontrado");

        plano.DefinirDados(
            entrada.Dados.Nome,
            entrada.Dados.CodigoRegistroAns);

        await GarantirUnicidadeAsync(
            plano,
            cancellationToken);

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