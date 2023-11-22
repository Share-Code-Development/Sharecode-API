using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Users.Create;

public record CreateUserCommand(
    string EmailAddress, 
    string FirstName, 
    string? MiddleName,
    string LastName,
    byte[]? Salt,
    byte[]? PasswordHash
    ) : IAppRequest<UserCreatedResponse>;