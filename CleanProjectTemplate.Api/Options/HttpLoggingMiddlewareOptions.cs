namespace CleanProjectTemplate.Api.Options;

public class HttpLoggingMiddlewareOptions
{
    public bool RequestMethod { get; init; }
    public bool RequestScheme { get; init; }
    public bool RequestPath { get; init; }
    public bool RequestQuery { get; init; }
    public bool RequestBody { get; init; }
    public bool ResponseBody { get; init; }
    public int MaxRequestBodyInKb { get; init; } = 32;
    public int MaxResponseBodyInKb { get; init; } = 32;
}
