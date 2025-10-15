using CleanProjectTemplate.Domain.Exceptions;

namespace CleanProjectTemplate.Api.ExceptionHandler;

public class HandledExceptionMessageProvider
{
    private readonly ILogger<HandledExceptionMessageProvider> _logger;
    private readonly IConfiguration _configuration;

    public HandledExceptionMessageProvider(IConfiguration configuration, ILogger<HandledExceptionMessageProvider> logger)
    {
        _logger = logger;
        _configuration = configuration.GetSection("ExceptionMessages");
    }

    public HandledExceptionModel GetMessage<T>(T exception)
        where T : HandledException
    {
        var details = _configuration.GetSection(exception.GetType().Name).Get<HandledExceptionModel>() ?? new HandledExceptionModel(string.Empty);

        string title;
        try
        {
            _logger.LogInformation("handling error of type '{type}' with title '{titlePattern}' and parameters: '{parameters}'", exception.GetType().Name, details.Title, string.Join(',', exception.Parameters));
            title = string.Format(details.Title, exception.Parameters);
        }
        catch
        {
            title = string.Empty;
        }

        string detail;
        try
        {
            _logger.LogInformation("handling error of type '{type}' with detail '{detailPattern}' and parameters: '{parameters}'", exception.GetType().Name, details.Detail, string.Join(',', exception.Parameters));

            detail = string.Format(details.Detail, exception.Parameters);
        }
        catch
        {
            detail = string.Empty;
        }

        return new(title, detail);
    }
}
