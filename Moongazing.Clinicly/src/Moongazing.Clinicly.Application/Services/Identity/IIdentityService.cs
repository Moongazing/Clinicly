using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Security.JWT;

namespace Moongazing.Clinicly.Application.Services.Identity;

public interface IIdentityService
{
    Task<AccessToken> CreateAccessToken(UserEntity user);
    Task<RefreshTokenEntity> CreateRefreshToken(UserEntity user, string ipAddress);
    Task<RefreshTokenEntity?> GetRefreshTokenByToken(string token);
    Task<RefreshTokenEntity> AddRefreshToken(RefreshTokenEntity refreshToken);
    Task DeleteOldRefreshTokens(Guid userId);
    Task RevokeDescendantRefreshTokens(RefreshTokenEntity refreshToken, string ipAddress, string reason);
    Task RevokeRefreshToken(RefreshTokenEntity refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null);
    Task<RefreshTokenEntity> RotateRefreshToken(UserEntity user, RefreshTokenEntity refreshToken, string ipAddress);

}