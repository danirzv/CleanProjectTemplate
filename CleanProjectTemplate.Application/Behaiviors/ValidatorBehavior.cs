using CleanProjectTemplate.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace CleanProjectTemplate.Application.Behaiviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidatorBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public ValidatorBehavior()
    {
        // For requests that does not have a validator.
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var result = _validator.Validate(request);
            if (!result.IsValid)    
            {
                throw new ValidationModelException(result);
            }
        }

        return next();
    }
}