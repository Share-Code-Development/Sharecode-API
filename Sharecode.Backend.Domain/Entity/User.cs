using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Entity;

public class User : BaseEntity
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
        get
        {
            return FullName.ToUpper();
        }
        private set
        {
            
        }
    }
    [Required]
    public byte[] Salt { get; set; }
    [Required]
    public byte[] PasswordHash { get; set; }
    [Required]
    public bool EmailVerified { get; set; }
    [Url]
    public string? ProfilePicture { get; set; }
    public AccountSetting AccountSetting { get; set; }
    [Required]
    public required AccountVisibility Visibility { get; set; }

}