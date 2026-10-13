using System.Text.Json.Serialization;
using EcoColeta.Api.Data;
using EcoColeta.Api.HealthChecks;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// ---------- MongoDB (configuração via appsettings ou variáveis de ambiente) ----------
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    return new MongoClient(settings);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return sp.GetRequiredService<IMongoClient>().GetDatabase(config.Database);
});

// ---------- Repositórios (uma collection por entidade) ----------
builder.Services
    .AddRepositorio<PontoColeta>("pontos_coleta")
    .AddRepositorio<Coleta>("coletas")
    .AddRepositorio<Usuario>("usuarios")
    .AddRepositorio<Notificacao>("notificacoes")
    .AddRepositorio<MaterialReciclavel>("materiais_reciclaveis");

// ---------- API, Swagger e Health Check ----------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoColeta API",
        Version = "v1",
        Description = "Cidades ESG Inteligentes — gestão de resíduos e reciclagem (FIAP · Navegando pelo mundo DevOps)."
    });
});

builder.Services.AddHealthChecks().AddCheck<MongoHealthCheck>("mongodb");

var app = builder.Build();

// ---------- Carga inicial de dados ----------
if (app.Configuration.GetValue("Seed:Enabled", true))
{
    await DataSeeder.SeedAsync(app.Services, app.Logger);
}

if (app.Configuration.GetValue("Swagger:Enabled", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocumentTitle = "EcoColeta API");
}

// Página inicial: identifica o ambiente (Staging/Production) e a versão implantada.
app.MapGet("/", (IWebHostEnvironment env, IConfiguration config) => Results.Ok(new
{
    aplicacao = "EcoColeta API",
    descricao = "Plataforma de gestão de resíduos e reciclagem para Cidades ESG Inteligentes",
    ambiente = env.EnvironmentName,
    versao = config["APP_VERSION"] ?? "local",
    dataHoraUtc = DateTime.UtcNow,
    documentacao = "/swagger",
    health = "/health"
})).ExcludeFromDescription();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            ambiente = app.Environment.EnvironmentName,
            checks = report.Entries.Select(e => new
            {
                nome = e.Key,
                status = e.Value.Status.ToString(),
                descricao = e.Value.Description
            })
        });
    }
});

app.MapControllers();

app.Run();

// Necessário para testes de integração (WebApplicationFactory).
public partial class Program { }
