using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Dto;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Users.Create;

public record UserCreatedResponse(Guid UserId, string FirstName, string? MiddleName, string LastName, string EmailAddress, bool EmailVerified, List<Meta> Metadata, AccountVisibility Visibility, AccountSettingDto Settings, DateTime Created, string RefreshToken, string AccessToken) : UserDto(UserId, FirstName, MiddleName, LastName, EmailAddress, EmailVerified, Metadata, Visibility, Settings, Created)
{
    public static UserCreatedResponse From(User user, AccessCredentials credentials)
    {
        return new(
            user.Id,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.EmailAddress,
            user.EmailVerified,
            user.Metadata,
            user.Visibility,
            AccountSettingDto.From(user),
            user.CreatedAt,
            credentials.RefreshToken,
            credentials.AccessToken
        );
    }
}