using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Dtos.Request;


public sealed record AtualizarBeneficiarioRequest(
    string? NomeCompleto,
    DateOnly DataNascimento,
    Guid PlanoId,
    StatusBeneficiario Status);
