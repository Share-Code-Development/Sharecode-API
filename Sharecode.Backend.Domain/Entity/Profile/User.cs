using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Events;

namespace Sharecode.Backend.Domain.Entity.Profile;

public class User : AggregateRootWithMetadata
{
    public User()
    {
    }

    public User(string emailAddress, string firstName, string? middleName, string lastName, byte[]? salt, byte[]? passwordHash)
    {
        EmailAddress = emailAddress;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Salt = salt;
        PasswordHash = passwordHash;
        AccountSetting = new AccountSetting();
        AccountSetting.User = this;
        EmailVerified = false;
        Visibility = AccountVisibility.Private;
    }
    

    [EmailAddress]
    [Length(minimumLength:5, maximumLength: 100)]
    [Required]
    public required string EmailAddress { get; init; }
    [Required]
    [Length(minimumLength: 3, maximumLength: 100)]
    public required string FirstName { get; set; }
    [Length(minimumLength: 3, maximumLength: 100)]
    public string? MiddleName { get; set; }
    [Required]
    [Length(minimumLength: 3, maximumLength: 100)]
    public required string LastName { get; set; }
    [NotMapped]
    public string FullName
    {
        get
        {
            if (string.IsNullOrEmpty(MiddleName))
                return $"{FirstName} {LastName}";
            
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }

    [Length(minimumLength: 9, maximumLength: 300)]
    public string NormalizedFullName
    {
        get
        {
            return FullName.ToUpper();
        }
        private set
        {
            
        }
    }
    public byte[]? Salt { get; set; }
    public byte[]? PasswordHash { get; set; }
    [Required]
    public bool EmailVerified { get; private set; }
    [Url]
    public string? ProfilePicture { get; set; }
    public AccountSetting AccountSetting { get; set; }
    [Required]
    public required AccountVisibility Visibility { get; set; }
    
    public override void RaiseCreatedEvent()
    {
        UserCreatedDomainEvent @event = UserCreatedDomainEvent.Create(this);
        RaiseDomainEvent(@event);
    }
}