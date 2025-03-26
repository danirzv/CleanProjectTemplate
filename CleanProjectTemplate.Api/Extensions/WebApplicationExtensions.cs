using Scalar.AspNetCore;

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
