using Moongazing.Clinicly.Domain.Entities;

namespace Moongazing.Kernel.Security.JWT;

public interface ITokenHelper
{
    AccessToken CreateToken(UserEntity user, IList<OperationClaimEntity> operationClaims);
    RefreshTokenEntity CreateRefreshToken(UserEntity user, string ipAddress);
}
