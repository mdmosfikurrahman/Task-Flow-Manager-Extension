using Microsoft.AspNetCore.Server.Kestrel.Core;
using Task_Flow_Manager_Extension.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.ConfigureProjectSettings();

var app = builder.Build();
app.UseProjectConfiguration();

app.Run();