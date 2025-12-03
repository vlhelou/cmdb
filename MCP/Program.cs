//using DotnetMCPServer.Shared.Clients;
//using DotnetMCPServer.Shared.Tools;
using McpServer.Clients;
using McpServer.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using System.Net.Http;


var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
    if (args != null)
    {
        config.AddCommandLine(args);
    }
});

var serverInfo = new Implementation { Name = "ModelContextProtocolClient", Version = "1.0.0" };

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMcpServer(mcp =>
    {
        mcp.ServerInfo = serverInfo;
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(ICTool).Assembly); ;

});

builder.ConfigureServices((hostContext, services) =>
{
    // Register IcClient with HttpClient
    services.AddHttpClient<IcClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5001/");
        client.DefaultRequestHeaders.Add("authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJ0ZXN0ZUBnbWFpbC5jb20iLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3NjQ3MjcxMTksImV4cCI6MTc2NDgxMzUxOSwiaWF0IjoxNzY0NzI3MTE5fQ.wcofCC-IXU3GOLRdpFVIVBdYPzErOr93UT_1qhxDqsM");
    });
});

var app = builder.Build();
await app.RunAsync();