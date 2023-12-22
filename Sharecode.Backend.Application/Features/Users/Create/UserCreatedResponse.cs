using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Dto;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Users.Create;

public class UserCreatedResponse : UserDto
{
    public UserCreatedResponse() { }

    public UserCreatedResponse(Guid userId, string firstName, string? middleName, string lastName, string emailAddress, bool emailVerified, Dictionary<string, object> metadata, AccountVisibility visibility, AccountSettingDto settings, DateTime created, string? profilePicture)
        : base(userId, firstName, middleName, lastName, emailAddress, emailVerified, metadata, visibility, settings, created, profilePicture) { }

    public new static UserCreatedResponse From(User user)
    {
        return new UserCreatedResponse(
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
            user.ProfilePicture
        );
    }
}