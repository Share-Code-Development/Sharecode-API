using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Application.Exceptions;
using ValidationException = Sharecode.Backend.Application.Exceptions.ValidationException;

namespace Sharecode.Backend.Application.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequestBase
{

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] results = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
        
        var errors = results 
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(errors => new ValidationError(
                errors.PropertyName,
                errors.ErrorMessage
            ));

        var validationErrors = errors.ToList();
        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);
        }

        var response = await next();
        return response;
    }
}