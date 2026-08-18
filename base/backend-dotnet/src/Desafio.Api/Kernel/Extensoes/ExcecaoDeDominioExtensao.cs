using System.Net;
using Desafio.Api.Kernel.Dominio.Excecoes;

namespace Desafio.Api.Kernel.Extensoes;

public static class ExcecaoDeDominioExtensions
{
    public static HttpStatusCode ParaStatusCode(this ExcecaoDeDominio excecao) =>
        excecao.Erro switch
        {
            TipoErro.ValidacaoInvalida => HttpStatusCode.BadRequest,
            TipoErro.NaoEncontrado => HttpStatusCode.NotFound,
            TipoErro.Conflito => HttpStatusCode.Conflict,
            TipoErro.NaoProcessavel => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };
}
