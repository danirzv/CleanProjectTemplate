using CleanProjectTemplate.Application.Exceptions;
using CleanProjectTemplate.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinimalHttp;

namespace CleanProjectTemplate.Api.ExceptionHandler;

public class GlobalExceptionHandler
{
    private const string UnhandledExceptionCode = "INTERNAL_SERVER_ERROR";

    public static Task OnExceptionAsync(HttpContext context)
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>()!;

        return HandleExceptionAsync(context, feature.Error);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<GlobalExceptionHandler>>();
        var messageProvider = context.RequestServices.GetRequiredService<HandledExceptionMessageProvider>();
        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        ProblemDetails problemDetails;
        LogLevel logLevel;

        switch (exception)
        {
            case ExternalProviderException externalProviderException:
                {
                    logLevel = LogLevel.Warning;

                    problemDetails = externalProviderException.Errors.Any() ? new ValidationProblemDetails(externalProviderException.Errors.ToDictionary()) : new ValidationProblemDetails();

                    problemDetails.Type = externalProviderException.Code;
                    problemDetails.Title =
                        string.IsNullOrEmpty(externalProviderException.Title) ? externalProviderException.Detail : externalProviderException.Title;
                    problemDetails.Detail = externalProviderException.Detail;
                    problemDetails.Status =
                        (int)externalProviderException.HttpStatusCode >= 200 && (int)externalProviderException.HttpStatusCode <= 299 ? 400 : (int)externalProviderException.HttpStatusCode;

                    break;
                }
            case ValidationModelException validationModelException:
                {
                    logLevel = LogLevel.Error;

                    var details = messageProvider.GetMessage(validationModelException);

                    problemDetails = new ValidationProblemDetails(validationModelException.Errors)
                    {
                        Type = "ValidationError",
                        Status = StatusCodes.Status400BadRequest,
                        Title = details.Title,
                        Detail = details.Detail,
                    };
                    break;
                }
            case HandledException handledException:
                {
                    logLevel = LogLevel.Error;

                    var details = messageProvider.GetMessage(handledException);
                    problemDetails = new ProblemDetails
                    {
                        Type = handledException.GetType().Name,
                        Status = StatusCodes.Status400BadRequest,
                        Title = details.Title,
                        Detail = details.Detail,
                    };

                    break;
                }
            default:
                {
                    logLevel = LogLevel.Critical;

                    problemDetails = new ProblemDetails
                    {
                        Type = UnhandledExceptionCode,
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Unhandled Exception",
                        Detail = "an unhandled exception occured",
                    };
                    break;
                }
        }

        logger.Log(logLevel, exception, "Some exception occured");

        if (env.IsDevelopment())
        {
            AddDevelopmentExceptionDetails(problemDetails.Extensions, exception);
        }


        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status.Value;

        switch (problemDetails)
        {
            case ValidationProblemDetails validationProblem:
                await context.Response.WriteAsJsonAsync(validationProblem);
                break;
            default:
                await context.Response.WriteAsJsonAsync(problemDetails);
                break;
        }
    }

    private static void AddDevelopmentExceptionDetails(IDictionary<string, object> dictionary, Exception exception)
    {
        dictionary = new Dictionary<string, object>();

        dictionary.Add("systemMessage", exception.Message);
        dictionary.Add("stackTrace", exception.StackTrace);

        if (exception.InnerException != null)
        {
            var innerData = new Dictionary<string, object>();
            dictionary.Add("inner", innerData);
            AddDevelopmentExceptionDetails(innerData, exception.InnerException);
        }
    }
}