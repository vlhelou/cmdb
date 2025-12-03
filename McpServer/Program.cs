using McpServer.Clients;
using McpServer.Tools;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);
var serverInfo = new Implementation { Name = "ModelContextProtocolClient", Version = "1.0.0" };
builder.Services.AddMcpServer(mcp =>
    {
        mcp.ServerInfo = serverInfo;
    })
    .WithHttpTransport()
    .WithToolsFromAssembly(typeof(ICTool).Assembly);

builder.Services.AddHttpClient<IcClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJ0ZXN0ZUBnbWFpbC5jb20iLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3NjQ3MjcxMTksImV4cCI6MTc2NDgxMzUxOSwiaWF0IjoxNzY0NzI3MTE5fQ.wcofCC-IXU3GOLRdpFVIVBdYPzErOr93UT_1qhxDqsM");
});

// Add services to the container.


var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapMcp();

app.Run();
