using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace OTPAuthorizer.Shared.Authentications.ApiKeyAuthentication;

public class ApiKeyAuthenticateHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ApiKey";
    public const string PolicyName = "ApiKeyPolicy";
    
    private const string HeaderName = "X-Api-Key";
    
    private readonly List<string> _apiKeys=[];

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var extracted))
            return Task.FromResult(AuthenticateResult.Fail("API Key missing"));

        if (!IsValid(extracted!))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, extracted!)
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal,SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private bool IsValid(string apiKey) => _apiKeys.Contains(apiKey);
}