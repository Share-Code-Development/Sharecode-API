using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Users.Create;

public record CreateUserCommand(
    string? EmailAddress, 
    string? FirstName, 
    string? MiddleName,
    string? LastName,
    string? Password,
    string? ProfilePicture
    ) : IAppRequest<UserCreatedResponse>;