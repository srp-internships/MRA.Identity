using System.Security.Cryptography;
using MRA.Identity.Application.Common.Interfaces.Services;

namespace MRA.Identity.Application.Services;

public class CryptoStringService : ICryptoStringService
{
    // ReSharper disable once InconsistentNaming
    private const string ALLOWED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_=/&";

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
            chars[i] = ALLOWED_CHARS[data[i] % ALLOWED_CHARS.Length];
        }

        return new string(chars);
    }
}