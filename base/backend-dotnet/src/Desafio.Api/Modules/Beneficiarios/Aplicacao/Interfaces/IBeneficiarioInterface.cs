using Desafio.Api.Kernel.Aplicacao.Comum;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario;
using Desafio.Api.Modules.Beneficiarios.Dominio.Entidades.Beneficiario.Enums;

namespace Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;

public interface IBeneficiarioRepositorio
{
    Task<Beneficiario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    // Ignora o filtro de exclusão lógica: CPF não pode ser reaproveitado
    // mesmo depois do beneficiário ser excluído.
    Task<bool> CpfEstaEmUsoAsync(string cpf, CancellationToken cancellationToken);

    Task<ResultadoPaginado<Beneficiario>> ListarAsync(
        int pagina,
        int tamanho,
        StatusBeneficiario? status,
        Guid? planoId,
        CancellationToken cancellationToken);

    void Adicionar(Beneficiario beneficiario);

    Task SalvarAsync(CancellationToken cancellationToken);
}
