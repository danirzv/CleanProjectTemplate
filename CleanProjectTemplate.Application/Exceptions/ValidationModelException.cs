using System.Text;
using CleanProjectTemplate.Domain.Exceptions;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CleanProjectTemplate.Application.Exceptions;


public class ValidationModelException(ModelStateDictionary errors) : HandledException(GenerateMessage(errors))
{
    public ValidationModelException(ValidationResult result) : this(GenerateModelStateDictionary(result))
    { }

    public ModelStateDictionary Errors { get; } = errors;

    private static ModelStateDictionary GenerateModelStateDictionary(ValidationResult result)
    {
        var modelState = new ModelStateDictionary();
        result.AddToModelState(modelState, string.Empty);
        return modelState;
    }

    private static string GenerateMessage(ModelStateDictionary errors)
    {
        var stringBuilder = new StringBuilder();

        foreach (var error in errors)
        {
            stringBuilder.Append(error.Key);
            stringBuilder.Append('=');
            foreach (var modelError in error.Value.Errors)
            {
                stringBuilder.Append(modelError.ErrorMessage);
                stringBuilder.Append(' ');
            }

            stringBuilder.Append(' ');
        }

        return stringBuilder.ToString();
    }
}
