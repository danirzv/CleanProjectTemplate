using CleanProjectTemplate.Api.Options;
using Microsoft.AspNetCore.HttpLogging;

namespace CleanProjectTemplate.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly HttpLoggingMiddlewareOptions DefaultHttpLoggingMiddlewareOptions = new();
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenApi()
            .AddApplicationHttpLogging(configuration);

        return services;
    }

    public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<Foo>(configuration.GetSection("Bar"));

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
}
