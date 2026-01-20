using OTPAuthorizer.Shared.Otp;
using OTPAuthorizer.Shared.Otp.Generators;

namespace OTPAuthorizer.Endpoints;

public static class OtpGenerateEndpoint
{
    public static RouteGroupBuilder MapOtpGenerateEndpoint(this RouteGroupBuilder routeGroupBuilder)
    {

        routeGroupBuilder.MapPost("{channel}/generate", async (
            string channel,
            string client,
            IServiceProvider sp) =>
        {
            IOtpGenerator svc = channel switch
            {
                SmsOtpDecorator.Channel => sp.GetRequiredService<SmsOtpDecorator>(),
                EmailOtpDecorator.Channel => sp.GetRequiredService<EmailOtpDecorator>(),
                _ => throw new ArgumentException("Invalid channel")
            };

            var otp = await svc.GenerateAsync(channel, client);
            return Results.Ok(otp);
        });
        
        return routeGroupBuilder;
    }
}