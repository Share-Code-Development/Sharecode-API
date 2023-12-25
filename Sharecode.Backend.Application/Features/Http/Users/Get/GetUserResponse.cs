using Sharecode.Backend.Domain.Dto;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Http.Users.Get;

public class GetUserResponse : UserDto
{
    public GetUserResponse() { }

    public GetUserResponse(Guid userId, string firstName, string? middleName, string lastName, string emailAddress, bool emailVerified, Dictionary<string, object> metadata, AccountVisibility visibility, AccountSettingDto? settings, DateTime created, string? profilePicture, HashSet<Permission> permissions)
        : base(userId, firstName, middleName, lastName, emailAddress, emailVerified, metadata, visibility, settings, created, profilePicture, permissions) { }

    public new static GetUserResponse From(User user)
    {
        return new GetUserResponse(
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
            user.ProfilePicture,
            user.Permissions
        );
    }
}
