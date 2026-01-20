using Microsoft.AspNetCore.Authentication;
using OTPAuthorizer.Endpoints;
using OTPAuthorizer.Shared.Authentications.ApiKeyAuthentication;
using OTPAuthorizer.Shared.Otp;
using OTPAuthorizer.Shared.Otp.Generators;
using OTPAuthorizer.Shared.Otp.Stores;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.Configure<OtpOptions>(builder.Configuration.GetSection("Otp"));

// Services
builder.Services.AddOpenApi();
    
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis") ?? throw new NullReferenceException("Redis Connection String")));

builder.Services.AddScoped<IOtpStore, RedisOtpStore>();
builder.Services.AddScoped<IOtpProtector, Sha256OtpProtector>();
builder.Services.AddScoped<OtpGenerator>();

// Authentication
builder.Services.AddAuthentication(ApiKeyAuthenticateHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticateHandler>(
        ApiKeyAuthenticateHandler.SchemeName, _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ApiKeyAuthenticateHandler.PolicyName, policy =>
    {
        policy.AddAuthenticationSchemes(ApiKeyAuthenticateHandler.SchemeName)
            .RequireAuthenticatedUser();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGroup("otp")
    .MapOtpGenerateEndpoint()
    .MapOtpVerifyEndpoint()
    .RequireAuthorization(ApiKeyAuthenticateHandler.PolicyName);


app.UseHttpsRedirection();


app.Run();