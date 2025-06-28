namespace Moongazing.Kernel.Security.OtpAuthenticator;

public interface IOtpAuthenticatorHelper
{
    Task<byte[]> GenerateSecretKeyAsync();
    Task<string> ConvertSecretKeyToStringAsync(byte[] secretKey);
    Task<bool> VerifyCodeAsync(byte[] secretKey, string code);
}