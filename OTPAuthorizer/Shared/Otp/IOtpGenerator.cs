namespace OTPAuthorizer.Shared.Otp;

public interface IOtpGenerator
{
    Task<OtpDto> GenerateAsync(string channel, string client);
    Task<bool> VerifyAsync(string channel, string client, string otp);
}

public record OtpDto(string Channel,
    string Client,
    string Code,
    int Length,
    DateTime ExpireIn,
    int ExpirationInMinutes);