using FluentValidation;
using Sharecode.Backend.Application.Features.Http.Users.Create;
using Sharecode.Backend.Application.Features.Http.Users.Delete;
using Sharecode.Backend.Application.Features.Http.Users.ForgotPassword;
using Sharecode.Backend.Application.Features.Http.Users.Login;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.Delete;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.List;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.Upsert;
using Sharecode.Backend.Application.Features.Http.Users.TagSearch;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Utilities.MetaKeys;

namespace Sharecode.Backend.Api.Validators.User;

public class UserValidator : AbstractValidator<CreateUserCommand>
{
    public UserValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName).NotEmpty();

        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Password).NotNull();
    }
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address should be present");
    }
}

public class LoginUserCommandValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotNull()
            .EmailAddress()
            .When(x => x.Type == AuthorizationType.Regular);

        RuleFor(x => x.Password)
            .NotNull()
            .When(x => x.Type == AuthorizationType.Regular);

        RuleFor(x => x.IdToken)
            .NotNull()
            .When(x => x.Type == AuthorizationType.Google);
    }
}

public class UserListTagQueryValidator : AbstractValidator<SearchUsersForTagCommand>
{
    public UserListTagQueryValidator()
    {
        RuleFor(x => x.SearchQuery)
            .MinimumLength(3)
            .WithMessage($"The user search should contain a minimum of 3 characters to search");
    }
}


public class ListUserMetadataQueryValidator : AbstractValidator<ListUserMetadataQuery>
{
    private static readonly HashSet<string> RestrictedKeys =
    [
        MetaKeys.UserKeys.RecentlyVisitedSnippets.Key
    ];

    public ListUserMetadataQueryValidator()
    {
        RuleFor(x => x.Queries)
            .Must(queries => queries.Any())
            .WithMessage("Queries should not be empty");
        
        RuleFor(x => x.Queries)
            .Must(queries => !queries.Any(key => RestrictedKeys.Contains(key)))
            .WithMessage(query => $"Queries contain restricted keys: {string.Join(", ", query.Queries.Where(key => RestrictedKeys.Contains(key)))}");
        
        RuleFor(x => x.Queries)
            .Must(queries => queries.All(key => key.StartsWith("FE")))
            .WithMessage(query => $"External metadata(s) should start with FE [For example: FE_userPassed], Invalid key(s): {string.Join(", ", query.Queries.Where(key => key.StartsWith("FE_")))}");

        RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage("Please ensure a proper user id in the query");
    }
}

public class UpsertUserMetadataCommandValidator : AbstractValidator<UpsertUserMetadataCommand>
{
    public UpsertUserMetadataCommandValidator()
    {
        RuleFor(x => x.MetaDictionary)
            .Must(queries => queries.Any())
            .WithMessage("External metadata(s) should not be empty");
        
        RuleFor(x => x.MetaDictionary)
            .Must(metaDic => metaDic.Keys.All(x => x.StartsWith("FE_")))
            .WithMessage(query => $"External metadata(s) should start with FE [For example: FE_userPassed], Invalid key(s): {string.Join(", ", query.MetaDictionary.Keys.Where(key => key.StartsWith("FE_")))}");
    }
}

public class DeleteUserMetadataCommandValidator : AbstractValidator<DeleteUserMetadataCommand>
{
    public DeleteUserMetadataCommandValidator()
    {
        RuleFor(x => x.Keys)
            .Must(queries => queries.Any())
            .WithMessage("External metadata(s) should not be empty");
        
        RuleFor(x => x.Keys)
            .Must(metaDic => metaDic.All(x => x.StartsWith("FE_")))
            .WithMessage(query => $"External metadata(s) should start with FE [For example: FE_userPassed], Invalid key(s): {string.Join(", ", query.Keys.Where(key => key.StartsWith("FE_")))}");
    }
}