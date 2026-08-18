using System.Net;
using Desafio.Domain.Compartilhado.Excecoes;

namespace Desafio.Api.Extensoes;

public static class ExcecaoDeDominioExtensao
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
