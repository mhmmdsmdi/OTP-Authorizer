namespace OTPAuthorizer.Shared.Otp;

public class OtpOptions
{
    public bool EncryptBeforeStore { get; set; }
    public Dictionary<string, OtpChannelOptions> Channels { get; set; } = new();
}