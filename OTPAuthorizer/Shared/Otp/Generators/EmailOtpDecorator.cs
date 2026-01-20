namespace OTPAuthorizer.Shared.Otp.Generators;

public class EmailOtpDecorator(OtpGenerator inner) : IOtpGenerator
{
    public const string Channel = "email";
    
    public async  Task<OtpDto> GenerateAsync(string channel, string client)
    {
        var otp = await inner.GenerateAsync(channel, client);
        
        // Publish notify

        return otp;
    }

    public  Task<bool> VerifyAsync(string channel, string client, string otp)
        => inner.VerifyAsync(channel, client, otp);
}