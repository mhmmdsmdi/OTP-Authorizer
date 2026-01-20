using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace OTPAuthorizer.Shared.Otp.Generators;

public class OtpGenerator(
    IOtpStore store,
    IOtpProtector protector,
    IOptions<OtpOptions> options)
    : IOtpGenerator
{
    private readonly OtpOptions _options = options.Value;

    public async Task<OtpDto> GenerateAsync(string channel, string client)
    {
        var ch = _options.Channels[channel];

        string otp;
        do
        {
            otp = GenerateCode(ch.CodeLength, ch.StartWithZero);
        }
        while (HasTooManyRepeatedDigits(otp, ch.MaxRepeatedDigits));

        var optToStore = _options.EncryptBeforeStore
            ? protector.Protect(otp)
            : otp;

        var expirationUtc = DateTime.UtcNow.AddMinutes(ch.ExpirationInMinutes);
        
        await store.SaveAsync(
            $"{channel}:{client}",
            optToStore,
            expirationUtc);

        return new OtpDto(channel,
            client,
            otp,
            ch.CodeLength,
            expirationUtc,
            ch.ExpirationInMinutes);
    }

    public async Task<bool> VerifyAsync(string channel, string client, string otp)
    {
        var stored = await store.GetAsync($"{channel}:{client}");
        if (stored == null) return false;

        var compare = _options.EncryptBeforeStore
            ? protector.Protect(otp)
            : otp;

        if (stored != compare) return false;

        await store.RemoveAsync($"{channel}:{client}");
        return true;
    }

    private static string GenerateCode(int len, bool startWithZero)
    {
        var min = startWithZero ? 0 : (int)Math.Pow(10, len - 1);
        var max = (int)Math.Pow(10, len) - 1;
        return RandomNumberGenerator.GetInt32(min, max).ToString($"D{len}");
    }

    private static bool HasTooManyRepeatedDigits(string code, int max)
    {
        return code
            .GroupBy(x => x)
            .Any(g => g.Count() > max);
    }
}