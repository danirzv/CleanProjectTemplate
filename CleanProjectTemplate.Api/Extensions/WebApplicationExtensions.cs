using Scalar.AspNetCore;
using Serilog;
using Serilog.Enrichers.Span;

namespace CleanProjectTemplate.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupPipeline(this WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.AddScalar();
        }

        app.UseHttpLogging();

        app.UseHttpsRedirection();

        return app;
    }

    public static Serilog.ILogger SetupSerilog(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.serilog.json", optional: false, reloadOnChange: true);

        var serilogLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithSpan()
            .CreateLogger();

        builder.Host.UseSerilog(serilogLogger, true);

        return serilogLogger;
    }

    private static void AddScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapGet("/", context => {
            context.Response.Redirect("scalar");
            return Task.CompletedTask;
        });
    }
}
