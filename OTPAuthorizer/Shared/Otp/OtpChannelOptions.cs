namespace OTPAuthorizer.Shared.Otp;

public class OtpChannelOptions
{
    public int CodeLength { get; set; }
    public bool StartWithZero { get; set; }
    public int MaxRepeatedDigits { get; set; }
    public int ExpirationInMinutes { get; set; }
}