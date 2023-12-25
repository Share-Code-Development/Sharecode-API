using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Create;

public record CreateUserCommand(
    string? EmailAddress, 
    string? FirstName, 
    string? MiddleName,
    string? LastName,
    string? Password,
    string? ProfilePicture
    ) : IAppRequest<UserCreatedResponse>;