using Desafio.Application.Planos.Validadores;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desafio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CriarPlanoRequestValidador>();
        services.AddUseCases();
        return services;
    }

    private static void AddUseCases(this IServiceCollection services)
    {
        var tipos = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(tipo => tipo is { IsClass: true, IsAbstract: false, IsPublic: true }
                           && ImplementaUseCase(tipo));

        foreach (var tipo in tipos)
        {
            services.AddScoped(tipo);
        }
    }

    private static bool ImplementaUseCase(Type tipo) =>
        tipo.GetInterfaces().Any(contrato =>
            contrato.IsGenericType &&
            contrato.GetGenericTypeDefinition() is var definicao &&
            (definicao == typeof(Compartilhado.IUseCase<,>)
             || definicao == typeof(Compartilhado.IUseCaseSemSaida<>)));
}
