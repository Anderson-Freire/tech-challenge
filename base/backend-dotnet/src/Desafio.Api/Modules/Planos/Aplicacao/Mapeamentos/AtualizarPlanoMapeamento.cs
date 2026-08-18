using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;

public static class AtualizarPlanoMapeamento
{
    public static void Aplicar(
        Plano plano,
        AtualizarPlanoRequest request)
    {
        plano.DefinirDados(
            request.Nome,
            request.CodigoRegistroAns);
    }
}