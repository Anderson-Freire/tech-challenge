using Desafio.Api.Modules.Planos.Aplicacao.Dtos.Request;
using Desafio.Api.Modules.Planos.Dominio.Entidades.Plano;

namespace Desafio.Api.Modules.Planos.Aplicacao.Mapeamentos;

public static class CriarPlanoMapeamento
{
    public static Plano ParaEntidade(CriarPlanoRequest request) =>
        new(
            request.Nome,
            request.CodigoRegistroAns);
}