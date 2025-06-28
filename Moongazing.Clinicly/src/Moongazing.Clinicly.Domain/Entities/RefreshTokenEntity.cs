using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Domain.Entities;

public class RefreshTokenEntity : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }

    public virtual UserEntity User { get; set; } = null!;

    public RefreshTokenEntity()
    {
        Token = string.Empty;
        CreatedByIp = string.Empty;
    }

    public RefreshTokenEntity(Guid userId, string token, DateTime expires, string createdByIp)
    {
        UserId = userId;
        Token = token;
        Expires = expires;
        CreatedByIp = createdByIp;
    }

    public RefreshTokenEntity(Guid id, Guid userId, string token, DateTime expires, string createdByIp)
        : base()
    {
        Id = id;
        UserId = userId;
        Token = token;
        Expires = expires;
        CreatedByIp = createdByIp;
    }
}