using MediatR;

namespace Sharecode.Backend.Application.Users.Create;

public record CreateUserCommand(
    string EmailAddress, 
    string FirstName, 
    string? MiddleName,
    string LastName,
    byte[]? Salt,
    byte[]? PasswordHash
    ) : IRequest;