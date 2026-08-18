
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Response;

public sealed record BeneficiarioResponse(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    StatusBeneficiario Status,
    Guid PlanoId,
    DateTime DataCadastro);