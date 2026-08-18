using Desafio.Api.Api.Comum;
using Desafio.Api.Api.Middlewares;
using Desafio.Api.Api.Serializacao;
using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Infraestrutura.ConfiguracaoBancoDeDados;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.CasosDeUso;
using Desafio.Api.Modules.Beneficiarios.Aplicacao.Interfaces;
using Desafio.Api.Modules.Beneficiarios.Infraestrutura.Repositorios;
using Desafio.Api.Modules.Planos.Aplicacao.CasosDeUso;
using Desafio.Api.Modules.Planos.Aplicacao.Interfaces;
using Desafio.Api.Modules.Planos.Aplicacao.Validadores;
using Desafio.Api.Modules.Planos.Infraestrutura.Repositorios;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

const string PoliticaDaWeb = "web";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

var connectionString = builder.Configuration.GetConnectionString("Postgres");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'Postgres' não foi configurada.");
}

builder.Services.AddDbContext<AppDbContext>(opcoes =>
    opcoes.UseNpgsql(connectionString));

builder.Services.AddScoped<IPlanoRepositorio, PlanoRepositorio>();
builder.Services.AddScoped<IBeneficiarioRepositorio, BeneficiarioRepositorio>();

builder.Services.AddScoped<ListarPlanosCasoDeUso>();
builder.Services.AddScoped<ObterPlanoCasoDeUso>();
builder.Services.AddScoped<CriarPlanoCasoDeUso>();
builder.Services.AddScoped<AtualizarPlanoCasoDeUso>();
builder.Services.AddScoped<ExcluirPlanoUseCase>();

builder.Services.AddScoped<ListarBeneficiariosCasoDeUso>();
builder.Services.AddScoped<ObterBeneficiarioCasoDeUso>();
builder.Services.AddScoped<CriarBeneficiarioCasoDeUso>();
builder.Services.AddScoped<AtualizarBeneficiarioCasoDeUso>();
builder.Services.AddScoped<ExcluirBeneficiarioCasoDeUso>();

builder.Services.AddValidatorsFromAssemblyContaining<CriarPlanoRequestValidador>();

builder.Services.AddCors(opcoes => opcoes.AddPolicy(
    PoliticaDaWeb,
    politica => politica
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services
    .AddControllers()
    .AddJsonOptions(opcoes =>
        JsonPadrao.Aplicar(opcoes.JsonSerializerOptions));

builder.Services.Configure<ApiBehaviorOptions>(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var detalhes = contexto.ModelState
            .Where(entrada => entrada.Value is { Errors.Count: > 0 })
            .Select(entrada => new DetalheErro(
                NormalizarCampo(entrada.Key),
                "invalido"))
            .ToList();

        var resposta = Resposta<object?>.Falha(
            new ValidacaoExcecao(
                "Corpo da requisição inválido",
                detalhes));

        return new ObjectResult(resposta)
        {
            StatusCode = (int)resposta.CodigoStatus
        };
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TratamentoDeExcecaoMiddleware>();

app.UseCors(PoliticaDaWeb);

app.UseSwagger();
app.UseSwaggerUI(opcoes =>
    opcoes.RoutePrefix = "swagger");

app.MapControllers();

await PrepararBancoAsync(app);

app.Run();

static string NormalizarCampo(string chave) =>
    chave.StartsWith("$.", StringComparison.Ordinal)
        ? chave[2..]
        : chave;

static async Task PrepararBancoAsync(WebApplication app)
{
    const int TentativasMaximas = 10;

    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    for (var tentativa = 1; ; tentativa++)
    {
        try
        {
            await using var escopo = app.Services.CreateAsyncScope();

            var db = escopo.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();

            // await CargaInicial.AplicarAsync(db);

            logger.LogInformation(
                "Banco preparado na tentativa {Tentativa}",
                tentativa);

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

public partial class Program;