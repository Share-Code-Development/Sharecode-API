using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Dto;

public class UserDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public bool EmailVerified { get; set; }
    public List<Meta> Metadata { get; set; }
    public AccountVisibility Visibility { get; set; }
    public AccountSettingDto? Settings { get; set; }
    public DateTime Created { get; set; }

    public UserDto() { }

    public UserDto(Guid userId, string firstName, string? middleName, string lastName, string emailAddress, bool emailVerified, List<Meta> metadata, AccountVisibility visibility, AccountSettingDto? settings, DateTime created)
    {
        UserId = userId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        EmailAddress = emailAddress;
        EmailVerified = emailVerified;
        Metadata = metadata;
        Visibility = visibility;
        Settings = settings;
        Created = created;
    }

    public static UserDto From(User user)
    {
        return new UserDto(
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

public sealed class AccountSettingDto
{
    public bool AllowTagging { get; set; }
    public List<Meta> Metadata { get; set; }

    public AccountSettingDto() { }

    public AccountSettingDto(bool allowTagging, List<Meta> metadata)
    {
        AllowTagging = allowTagging;
        Metadata = metadata;
    }

    public static AccountSettingDto From(User user)
    {
        AccountSetting setting = user.AccountSetting;
        return setting == null ? null : new AccountSettingDto(setting.AllowTagging, setting.Metadata);
    }
}
