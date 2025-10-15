using CleanProjectTemplate.Api.Authentication;
using CleanProjectTemplate.Api.Authorization;
using CleanProjectTemplate.Api.ExceptionHandler;
using CleanProjectTemplate.Api.Options;
using Microsoft.AspNetCore.HttpLogging;

namespace CleanProjectTemplate.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly HttpLoggingMiddlewareOptions DefaultHttpLoggingMiddlewareOptions = new();

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddControllers().Services
            .AddOpenApi()
            .AddApplicationHttpLogging(configuration)
            .AddExceptionHandling(configuration)
            .AddCleanProjectAuthentication()
            .AddCleanProjectAuthorization();

        return services;
    }

    public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<HttpLoggingMiddlewareOptions>(configuration.GetSection("HttpLoggingMiddleware"));

        return services;
    }

    private static IServiceCollection AddApplicationHttpLogging(this IServiceCollection services, IConfiguration configuration)
        => services.AddHttpLogging(cfg =>
        {
            var options = configuration.GetSection("HttpLoggingMiddleware").Get<HttpLoggingMiddlewareOptions>() ?? DefaultHttpLoggingMiddlewareOptions;

            var loggingFields = HttpLoggingFields.None;

            if (options.RequestMethod)
            {
                loggingFields |= HttpLoggingFields.RequestMethod;
            }

            if (options.RequestScheme)
            {
                loggingFields |= HttpLoggingFields.RequestScheme;
            }

            if (options.RequestPath)
            {
                loggingFields |= HttpLoggingFields.RequestPath;
            }

            if (options.RequestQuery)
            {
                loggingFields |= HttpLoggingFields.RequestQuery;
            }

            if (options.ResponseBody)
            {
                loggingFields |= HttpLoggingFields.ResponseBody;
            }

            cfg.LoggingFields = loggingFields;
            cfg.RequestBodyLogLimit = options.MaxRequestBodyInKb * 1024;
            cfg.ResponseBodyLogLimit = options.MaxResponseBodyInKb * 1024;
        });

    private static IServiceCollection AddExceptionHandling(this IServiceCollection services, ConfigurationManager configuration)
    {
        configuration.AddJsonFile("appsettings.exceptions.json", optional: false, reloadOnChange: true);

        services.AddSingleton<HandledExceptionMessageProvider>();

        return services;
    }

    private static IServiceCollection AddCleanProjectAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            //TODO: Setup Authentication Methods here
            .AddCookie(Schemes.SampleScheme, options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        return services;
    }

    private static IServiceCollection AddCleanProjectAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            //TODO: Remove this
            options.AddPolicy(Policies.SomePolicy, policy => { policy.RequireAuthenticatedUser(); });

            //TODO: Setup Policies here
        });

        return services;
    }
}
