using Desafio.Api.Contratos;
using Desafio.Api.Middlewares;
using Desafio.Application;
using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Infrastructure;
using Microsoft.AspNetCore.Mvc;

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

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddCors(opcoes => opcoes.AddPolicy(
    PoliticaDaWeb,
    politica => politica
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services
    .AddControllers(opcoes => opcoes.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(opcoes => JsonPadrao.Aplicar(opcoes.JsonSerializerOptions));

builder.Services.Configure<ApiBehaviorOptions>(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var detalhes = contexto.ModelState
            .Where(entrada => entrada.Value is { Errors.Count: > 0 })
            .Select(entrada => new DetalheErro(NormalizarCampo(entrada.Key), "invalido"))
            .ToList();

        return new BadRequestObjectResult(
            ErroResponse.Validacao("Corpo da requisição inválido", detalhes));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<LogDeRequisicaoMiddleware>();
app.UseMiddleware<TratamentoDeExcecaoMiddleware>();
app.UseCors(PoliticaDaWeb);
app.UseSwagger();
app.UseSwaggerUI(opcoes => opcoes.RoutePrefix = "swagger");
app.MapControllers();

await app.Services.PrepararBancoAsync(app.Logger);

app.Run();

static string NormalizarCampo(string chave) =>
    chave.StartsWith("$.", StringComparison.Ordinal) ? chave[2..] : chave;

public partial class Program;
