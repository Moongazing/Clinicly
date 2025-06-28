namespace Moongazing.Kernel.Security.JWT;

public interface ITokenHelper
{
    AccessToken CreateToken(UserEntity user, IList<OperationClaimEntity> operationClaims, string cd_vbCassa);
    RefreshTokenEntity CreateRefreshToken(UserEntity user, string ipAddress);
}
