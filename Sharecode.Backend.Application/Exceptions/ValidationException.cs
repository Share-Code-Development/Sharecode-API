namespace Sharecode.Backend.Application.Exceptions;

public class ValidationException : Exception
{
    
    
    public ValidationException(IEnumerable<ValidationError> errors) : base($"An error has been occured!")
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; private set; }
}

public record ValidationError(string PropertyName, string ErrorMessage);