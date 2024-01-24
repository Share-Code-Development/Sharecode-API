using FluentValidation;
using FluentValidation.Results;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Http.Users.Create;

public class CreateUserCommandPersistenceValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandPersistenceValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.EmailAddress)
            .NotNull()
            .WithMessage($"Please provide a valid email address!")
            .CustomAsync(async (email, context, token) =>
            {
                // assume that user is your model object and you've populated EmailAddressState somewhere before this validation
                var emailState = await userRepository.IsEmailAddressUnique(email!, token);
                switch (emailState)
                {
                    case EmailState.Present:
                    context.AddFailure(new ValidationFailure("EmailAddress", "This email address has already been registered.") { ErrorCode = "020021" });
                    break;
                    case EmailState.Deleted:
                    context.AddFailure(new ValidationFailure("EmailAddress", "This email address has been deleted.") { ErrorCode = "020022" });
                    break;
                }
            });
    }
}