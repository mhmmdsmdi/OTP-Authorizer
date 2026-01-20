using StackExchange.Redis;

namespace OTPAuthorizer.Shared.Otp.Stores;

public class RedisOtpStore(IConnectionMultiplexer mux) : IOtpStore
{
    private readonly IDatabase _db = mux.GetDatabase();
    private const string PrefixKey = "opt:authorizer";
    
    public async Task SaveAsync(string key, string otp,DateTime expirationUtc)
        => await _db.StringSetAsync($"{PrefixKey}:{key}",otp,expirationUtc);

    public async Task<string?> GetAsync(string key)
        => await _db.StringGetAsync($"{PrefixKey}:{key}");

    public Task RemoveAsync(string key)
        => _db.KeyDeleteAsync($"{PrefixKey}:{key}");
}