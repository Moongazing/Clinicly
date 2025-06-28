using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Domain.Entities;

public class UserEntity : Entity<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }
    public bool Status { get; set; }
    public AuthenticatorType AuthenticatorType { get; set; }

    public virtual ICollection<UserOperationClaimEntity> UserOperationClaims { get; set; } = new HashSet<UserOperationClaimEntity>();
    public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new HashSet<RefreshTokenEntity>();
    public virtual ICollection<EmailAuthenticatorEntity> EmailAuthenticators { get; set; } = new HashSet<EmailAuthenticatorEntity>();
    public virtual ICollection<OtpAuthenticatorEntity> OtpAuthenticators { get; set; } = new HashSet<OtpAuthenticatorEntity>();



    public UserEntity()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PasswordHash = [];
        PasswordSalt = [];
    }

    public UserEntity(
        string firstName,
        string lastName,
        string email,
        byte[] passwordSalt,
        byte[] passwordHash,
        bool status,
        AuthenticatorType authenticatorType
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Status = status;
        AuthenticatorType = authenticatorType;
    }

    public UserEntity(
        Guid id,
        string firstName,
        string lastName,
        string email,
        byte[] passwordSalt,
        byte[] passwordHash,
        bool status,
        AuthenticatorType authenticatorType
    )
        : base()
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Status = status;
        AuthenticatorType = authenticatorType;
    }
}