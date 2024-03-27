using System.Security.Cryptography;
using MRA.Identity.Application.Common.Interfaces.Services;

namespace MRA.Identity.Application.Services;

public class CryptoStringService : ICryptoStringService
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_=/&";

    public string GetCryptoString(int length = 86)
    {
        var chars = new char[length];
        var data = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }

        for (var i = 0; i < length; i++)
        {
            chars[i] = AllowedChars[data[i] % AllowedChars.Length];
        }

        return new string(chars);
    }
}