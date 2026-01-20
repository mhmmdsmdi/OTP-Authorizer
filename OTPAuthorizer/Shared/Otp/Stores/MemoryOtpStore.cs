using Microsoft.Extensions.Caching.Memory;

namespace OTPAuthorizer.Shared.Otp.Stores;

public class MemoryOtpStore(IMemoryCache cache) : IOtpStore
{
    public Task SaveAsync(string key, string otp,DateTime expirationUtc)
    {
        cache.Set(key, otp, expirationUtc);
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string key)
    {
        cache.TryGetValue(key, out string? otp);
        return Task.FromResult(otp);
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}