using OTPAuthorizer.Shared.Otp;
using OTPAuthorizer.Shared.Otp.Generators;

namespace OTPAuthorizer.Endpoints;

public static class OtpVerifyEndpoint
{
    public static RouteGroupBuilder MapOtpVerifyEndpoint(this RouteGroupBuilder routeGroupBuilder)
    {
        
        routeGroupBuilder.MapPost("{channel}/verify", async (
            string channel,
            string client,
            string otp,
            IServiceProvider sp) =>
        {
            IOtpGenerator svc = channel switch
            {
                SmsOtpDecorator.Channel => sp.GetRequiredService<SmsOtpDecorator>(),
                EmailOtpDecorator.Channel => sp.GetRequiredService<EmailOtpDecorator>(),
                _ => throw new ArgumentException("Invalid channel")
            };

            var ok = await svc.VerifyAsync(channel,client, otp);
            return ok ? Results.Ok() : Results.Unauthorized();
        });
        
        return routeGroupBuilder;
    }
}