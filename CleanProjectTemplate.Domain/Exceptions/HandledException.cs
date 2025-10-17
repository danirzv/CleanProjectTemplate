namespace CleanProjectTemplate.Domain.Exceptions;

public abstract class HandledException(string message) : Exception(message)
{
    public string[] Parameters { get; protected set; } = [];
}
