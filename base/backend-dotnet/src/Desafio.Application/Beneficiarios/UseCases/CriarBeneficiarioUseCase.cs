using Desafio.Application.Beneficiarios.Dtos;
using Desafio.Application.Beneficiarios.Mapeamentos;
using Desafio.Application.Compartilhado;
using Desafio.Application.Compartilhado.Validacao;
using Desafio.Application.Planos;
using Desafio.Domain.Compartilhado.Excecoes;
using FluentValidation;

namespace Desafio.Application.Beneficiarios.UseCases;

public sealed class CriarBeneficiarioUseCase(
    IBeneficiarioRepositorio beneficiarioRepositorio,
    IPlanoRepositorio planoRepositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarBeneficiarioRequest> validador)
    : IUseCase<CriarBeneficiarioRequest, CriarBeneficiarioResponse>
{
    public async Task<CriarBeneficiarioResponse> ExecutarAsync(
        CriarBeneficiarioRequest entrada,
        CancellationToken cancellationToken)
    {
        await validador.ValidarAsync(entrada, cancellationToken);
        await GarantiaDePlanoExistente.GarantirAsync(
            planoRepositorio,
            entrada.PlanoId!.Value,
            cancellationToken);

        var beneficiario = BeneficiarioMapeamento.ParaEntidade(entrada);

        if (await beneficiarioRepositorio.CpfEstaEmUsoAsync(
                beneficiario.Cpf.Valor,
                cancellationToken))
        {
            throw new ConflitoExcecao(
                "CPF já cadastrado",
                [new DetalheErro(nameof(CriarBeneficiarioRequest.Cpf), "duplicado")]);
        }

        beneficiarioRepositorio.Adicionar(beneficiario);
        await unidadeDeTrabalho.SalvarAsync(cancellationToken);

        return CriarBeneficiarioResponse.De(beneficiario);
    }
}
