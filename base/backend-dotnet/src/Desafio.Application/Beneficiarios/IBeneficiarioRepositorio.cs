using Desafio.Application.Compartilhado;
using Desafio.Domain.Beneficiarios;

namespace Desafio.Application.Beneficiarios;

public interface IBeneficiarioRepositorio
{
    Task<Beneficiario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CpfEstaEmUsoAsync(string cpf, CancellationToken cancellationToken);

    Task<ResultadoPaginado<Beneficiario>> ListarAsync(
        int pagina,
        int tamanho,
        StatusBeneficiario? status,
        Guid? planoId,
        CancellationToken cancellationToken);

    void Adicionar(Beneficiario beneficiario);
}
