using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Dto;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Users.Get;

public class GetUserResponse : UserDto
{
    public GetUserResponse() { }

    public GetUserResponse(Guid userId, string firstName, string? middleName, string lastName, string emailAddress, bool emailVerified, List<Meta> metadata, AccountVisibility visibility, AccountSettingDto settings, DateTime created)
        : base(userId, firstName, middleName, lastName, emailAddress, emailVerified, metadata, visibility, settings, created) { }

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
            user.CreatedAt
        );
    }
}
