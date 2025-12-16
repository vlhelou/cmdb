using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OllamaSharp;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var strcn = Environment.GetEnvironmentVariable("CMDB_DB")?.Replace(":", "=") ?? "";
var verificaVetor = (Environment.GetEnvironmentVariable("vector") ?? string.Empty)=="true";

NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();


//var dataSourceBuilder = new NpgsqlDataSourceBuilder(strcn);
//dataSourceBuilder.EnableDynamicJson();
//dataSourceBuilder.UseVector();
//var dataSource = dataSourceBuilder.Build();
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
string urlEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 26)?.ValorTexto ?? "";
string modeloEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 27)?.ValorTexto ?? "";
bool ativoEmbed = valoresEmbed.FirstOrDefault(p => p.Id == 28)?.ValorBoleano ?? false;

if (ativoEmbed)
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

//var key = Convert.FromBase64String(builder.Configuration.GetValue<string>("jwt") ?? string.Empty);
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
