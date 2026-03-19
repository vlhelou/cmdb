using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OllamaSharp;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var strcn = Environment.GetEnvironmentVariable("CMDB_DB")?.Replace(":", "=") ?? "";
var verificaVetor = (Environment.GetEnvironmentVariable("vector") ?? string.Empty) == "true";

NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
builder.Services.AddDbContext<Cmdb.Model.Db>(opt =>
{
    opt.UseNpgsql(strcn.Replace(":", "="), options =>
    {
        options.EnableRetryOnFailure();

        options.UseVector();
    });
    opt.LogTo(Console.WriteLine);
});




#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var db = builder?.Services?.BuildServiceProvider().GetService<Cmdb.Model.Db>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'



if (db == null)
    throw new Exception("Erro ao conectar ao banco de dados");

var valoresEmbed = db.CorpConfiguracao.AsNoTracking().Where(p => p.Id == 26 || p.Id == 27 || p.Id == 28).ToList();
string urlEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 26)?.ValorTexto ?? "http://localhost:11434";
string modeloEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 27)?.ValorTexto ?? "";
bool ativoEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 28)?.ValorBoleano ?? false;

builder.Services.AddTransient<OllamaApiClient>(sp => new OllamaApiClient(uriString: urlEmbed, defaultModel: modeloEmbed));


string chaveJWT = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 16)?.ValorTexto ?? "";


builder!.Services.AddCors(options =>
{
    options.AddPolicy("Geral",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var key = Encoding.UTF8.GetBytes(chaveJWT);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

bool otlpAtivo = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 13)?.ValorBoleano ?? false;
if (otlpAtivo)
{
    string otlpEndpoint = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 29)?.ValorTexto ?? "";
    string otlpServico = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 30)?.ValorTexto ?? "otlp";


    var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: "Cmdb",
        serviceVersion: "1.1");

    builder.Logging.ClearProviders();

    //builder.Logging.AddOpenTelemetry(options =>
    //{
    //    options.IncludeFormattedMessage = true;
    //    options.IncludeScopes = true;
    //    options.AddOtlpExporter(otlpOptions =>
    //    {
    //        otlpOptions.Endpoint = new Uri(otlpEndpoint);
    //    }).SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Logging.Cmdb"));
    //});


    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(otlpServico))
        .WithMetrics(metrics =>
        {
            metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddNpgsqlInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            });


        })
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            });
        })
        .WithLogging(logging =>
        {
            logging.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            }).SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Logging.Cmdb"));
            logging.AddConsoleExporter();
        })
        .ConfigureResource(resouce => { 
            resouce.AddService(serviceName: "Cmdb", serviceVersion: "1.1");
        })
        ;


}

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseCors("Geral");
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.MapFallbackToFile("/index.html");

app.Run();
