namespace OTPAuthorizer.Shared.Otp;

public interface IOtpStore
{
    Task SaveAsync(string key, string otp,DateTime expirationUtc);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
}