using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Security.JWT;
using TokenOptions = Moongazing.Kernel.Security.JWT.TokenOptions;

namespace Moongazing.Clinicly.Application.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly ITokenHelper tokenHelper;
    private readonly TokenOptions tokenOptions;
    private readonly IUserOperationClaimRepository userOperationClaimService;

    public IdentityService(IUserOperationClaimRepository userOperationClaimService,
                           IRefreshTokenRepository refreshTokenRepository,
                           ITokenHelper tokenHelper,
                           IConfiguration configuration)
    {
        this.userOperationClaimService = userOperationClaimService;
        this.refreshTokenRepository = refreshTokenRepository;
        this.tokenHelper = tokenHelper;

        const string tokenOptionsConfigurationSection = "TokenOptions";
        tokenOptions = configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
    }
    public async Task<AccessToken> CreateAccessToken(UserEntity user)
    {
        IList<OperationClaimEntity> operationClaims = await userOperationClaimService.GetOperationClaimsByUserIdAsync(user.Id);
        AccessToken accessToken = tokenHelper.CreateToken(user, operationClaims);
        return accessToken;
    }
    public async Task<RefreshTokenEntity> AddRefreshToken(RefreshTokenEntity refreshToken)
    {
        RefreshTokenEntity addedRefreshToken = await refreshTokenRepository.AddAsync(refreshToken);
        return addedRefreshToken;
    }
    public async Task DeleteOldRefreshTokens(Guid userId)
    {
        List<RefreshTokenEntity> refreshTokens = await refreshTokenRepository
            .Query()
            .AsNoTracking()
            .Where(
                r =>
                    r.UserId == userId
                    && r.Revoked == null
                    && r.Expires >= DateTime.UtcNow
                    && r.CreatedDate!.Value.AddDays(tokenOptions.RefreshTokenTTL) <= DateTime.UtcNow
            )
            .ToListAsync();

        await refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshTokenEntity?> GetRefreshTokenByToken(string token)
    {
        RefreshTokenEntity? refreshToken = await refreshTokenRepository.GetAsync(r => r.Token == token);
        return refreshToken;
    }

    public async Task RevokeRefreshToken(RefreshTokenEntity refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await refreshTokenRepository.UpdateAsync(refreshToken);
    }



    public async Task<RefreshTokenEntity> RotateRefreshToken(UserEntity user, RefreshTokenEntity refreshToken, string ipAddress)
    {
        RefreshTokenEntity newRefreshToken = tokenHelper.CreateRefreshToken(user, ipAddress);
        await RevokeRefreshToken(refreshToken, ipAddress, reason: "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    public async Task RevokeDescendantRefreshTokens(RefreshTokenEntity refreshToken, string ipAddress, string reason)
    {
        RefreshTokenEntity? childToken = await refreshTokenRepository.GetAsync(predicate: r => r.Token == refreshToken.ReplacedByToken);

        if (childToken != null)
        {

            if (childToken.Revoked == null && childToken.Expires > DateTime.UtcNow)
            {
                await RevokeRefreshToken(childToken, ipAddress, reason);
            }
            else
            {
                await RevokeDescendantRefreshTokens(refreshToken: childToken, ipAddress, reason);
            }
        }
    }
    public Task<RefreshTokenEntity> CreateRefreshToken(UserEntity user, string ipAddress)
    {
        RefreshTokenEntity refreshToken = tokenHelper.CreateRefreshToken(user, ipAddress);
        return Task.FromResult(refreshToken);
    }
}