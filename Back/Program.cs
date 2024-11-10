using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var strcn = Environment.GetEnvironmentVariable("CMDB_DB") ?? "";
builder.Services.AddDbContext<Cmdb.Model.Db>(opt =>
{
    opt.UseNpgsql(strcn, options =>
    {
        options.EnableRetryOnFailure();
    });
    opt.LogTo(Console.WriteLine);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("Geral",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
