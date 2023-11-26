using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Utilities.RequestDetail;

namespace Sharecode.Backend.Domain.Entity.Profile;

public class User : AggregateRootWithMetadata
{
    public User()
    {
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
        get => FullName.ToUpper();
        private set
        {
            
        }
    }

    public DateTime LastLogin { get; private set; } = DateTime.UtcNow;
    public byte[]? Salt { get; set; }
    public byte[]? PasswordHash { get; set; }
    [Required]
    public bool EmailVerified { get; private set; }
    [Url]
    public string? ProfilePicture { get; set; }
    public AccountSetting AccountSetting { get; set; }
    [Required]
    public required AccountVisibility Visibility { get; set; }
    public bool Active { get; private set; }
    public string? InActiveReason { get; private set; }
    public bool AccountLocked { get; private set; } = false;
    private DateTime? LastUnsuccessfulLoginAttempt { get; set; } = null;
    private int FailedAttemptCount { get; set; }
        
    
    public override void RaiseCreatedEvent()
    {
        if(EmailVerified)
            return;
        
        UserCreatedDomainEvent @event = UserCreatedDomainEvent.Create(this);
        RaiseDomainEvent(@event);
    }

    public bool VerifyUser()
    {
        if(EmailVerified)
            return false;

        EmailVerified = true;
        RaiseDomainEvent(UserVerifiedDomainEvent.Create(this));
        return true;
    }

    public bool SetInActive(string reason)
    {
        if(!Active)
            return false;

        InActiveReason = reason;
        Active = false;
        RaiseDomainEvent(AccountSetInActiveDomainEvent.Create(this));
        return true;
    }

    public bool SetActive()
    {
        if (Active)
            return false;

        InActiveReason = null;
        Active = true;
        return true;
    }
    

    public void ResendEmailVerification()
    {
        if(EmailVerified)
            return;
        
        RaiseDomainEvent(UserCreatedDomainEvent.Create(this));
    }

    public bool RequestPasswordReset(bool verifyAccountStatus = true)
    {
        if (!Active)
            return false;

        RaiseDomainEvent(RequestPasswordResetDomainEvent.Create(this));
        return true;
    }

    public void SetUnsuccessfulLoginAttempt(IRequestDetail requestDetail)
    {
        //Assign if the user has not failed a single attempt
        //after sign up
        LastUnsuccessfulLoginAttempt ??= DateTime.UtcNow;

        var lastAttempt = LastUnsuccessfulLoginAttempt.Value;
        var hasCountExceeded = FailedAttemptCount >= 5;
        
        LastUnsuccessfulLoginAttempt = DateTime.UtcNow;
        FailedAttemptCount++;
        
        var minutes = (DateTime.UtcNow - lastAttempt).TotalMinutes;
        
        if (minutes <= 10 && hasCountExceeded)
        {
            AccountLocked = true;
            PasswordHash = null;
            Salt = null;
            RaiseDomainEvent(AccountLockedDomainEvent.Create(this, requestDetail));
        }
    }

    public void SetSuccessfulLoginAttempt(IRequestDetail requestDetail)
    {
        FailedAttemptCount = 0;
        LastLogin = DateTime.UtcNow;
    }

    public void UpdatePassword(byte[] passwordHash, byte[] salt, IRequestDetail detail)
    {
        bool wasAccountLocked = AccountLocked;

        PasswordHash = passwordHash;
        Salt = salt;
        
        RaiseDomainEvent(PasswordResetSuccessDomainEvent.Create(this, detail, wasAccountLocked));
        if (!AccountLocked) return;
        //Reset those parameters only if the account was locked and this is a reset
        LastUnsuccessfulLoginAttempt = null;
        FailedAttemptCount = 0;
        AccountLocked = false;
    }
}