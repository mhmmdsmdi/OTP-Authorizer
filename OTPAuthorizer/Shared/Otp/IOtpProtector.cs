using System.Security.Cryptography;
using System.Text;

namespace OTPAuthorizer.Shared.Otp;

public interface IOtpProtector
{
    string Protect(string input);
}

public class Sha256OtpProtector : IOtpProtector
{
    public string Protect(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}