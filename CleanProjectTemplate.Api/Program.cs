using CleanProjectTemplate.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSerilog();

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddApplicationConfigurations(builder.Configuration);

var app = builder.Build();

app.SetupPipeline(builder.Configuration);

app.MapGet("/Test", () => "Hello world!");  //TODO: just remove this

app.Run();