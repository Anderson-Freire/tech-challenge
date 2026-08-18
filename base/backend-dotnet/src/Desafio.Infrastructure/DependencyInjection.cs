using Desafio.Application.Beneficiarios;
using Desafio.Application.Compartilhado;
using Desafio.Application.Planos;
using Desafio.Infrastructure.Beneficiarios;
using Desafio.Infrastructure.Persistencia;
using Desafio.Infrastructure.Planos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Desafio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(opcoes => opcoes.UseNpgsql(connectionString));
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        services.AddScoped<IPlanoRepositorio, PlanoRepositorio>();
        services.AddScoped<IBeneficiarioRepositorio, BeneficiarioRepositorio>();
        services.AddScoped<IVerificadorDeBanco, VerificadorDeBanco>();
        return services;
    }

    public static async Task PrepararBancoAsync(
        this IServiceProvider services,
        ILogger logger)
    {
        const int TentativasMaximas = 10;

        for (var tentativa = 1; ; tentativa++)
        {
            try
            {
                await using var escopo = services.CreateAsyncScope();
                var db = escopo.ServiceProvider.GetRequiredService<AppDbContext>();

                await db.Database.MigrateAsync();
                await CargaInicial.AplicarAsync(db);

                logger.LogInformation("Banco preparado na tentativa {Tentativa}", tentativa);
                return;
            }
            catch (Exception excecao) when (tentativa < TentativasMaximas)
            {
                logger.LogWarning(
                    "Banco indisponível na tentativa {Tentativa} de {TentativasMaximas}: {Mensagem}",
                    tentativa,
                    TentativasMaximas,
                    excecao.Message);

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}
