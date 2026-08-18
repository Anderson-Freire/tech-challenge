using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Domain.Compartilhado.ObjetosDeValor;
using Desafio.Domain.Planos;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Desafio.Infrastructure.Persistencia;

internal static class ViolacaoDeUnicidade
{
    private const string CodigoPostgres = "23505";

    public static bool Eh(DbUpdateException excecao) =>
        excecao.InnerException is PostgresException postgres &&
        postgres.SqlState == CodigoPostgres;

    public static ConflitoExcecao ParaExcecao(DbUpdateException excecao)
    {
        var restricao = (excecao.InnerException as PostgresException)?.ConstraintName ?? string.Empty;

        if (restricao.Contains(nameof(Cpf), StringComparison.OrdinalIgnoreCase))
        {
            return new ConflitoExcecao(
                "CPF já cadastrado",
                [new DetalheErro(nameof(Cpf), "duplicado")]);
        }

        if (restricao.Contains(nameof(Plano.Nome), StringComparison.OrdinalIgnoreCase))
        {
            return new ConflitoExcecao(
                "Já existe plano cadastrado com esse valor",
                [new DetalheErro(nameof(Plano.Nome), "duplicado")]);
        }

        if (restricao.Contains(nameof(Plano.CodigoRegistroAns), StringComparison.OrdinalIgnoreCase))
        {
            return new ConflitoExcecao(
                "Já existe plano cadastrado com esse valor",
                [new DetalheErro(nameof(Plano.CodigoRegistroAns), "duplicado")]);
        }

        return new ConflitoExcecao("Já existe registro cadastrado com esse valor");
    }
}
