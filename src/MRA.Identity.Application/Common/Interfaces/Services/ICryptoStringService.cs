namespace MRA.Identity.Application.Common.Interfaces.Services;

public interface ICryptoStringService
{
    string GetCryptoString(int length = 86);
}