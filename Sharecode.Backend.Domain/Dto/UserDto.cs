using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Dto;

public sealed record AccountSettingDto(
    bool AllowTagging,
    Dictionary<string, object> Metadata
)
{
    public static AccountSettingDto From(User user)
    {
        AccountSetting setting = user.AccountSetting;
        return new AccountSettingDto(
                setting.AllowTagging,
                setting.Metadata
                );
    }
}

public record UserDto(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string EmailAddress,
    bool EmailVerified,
    Dictionary<string, object> Metadata,
    AccountVisibility Visibility,
    AccountSettingDto Settings,
    DateTime Created
)
{
    public static UserDto From(User user)
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
            user.CreatedAt
        );
    }
}