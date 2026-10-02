using System.Security.Cryptography;
using System.Text;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.Authentication;

public class TokenHasher : ITokenHasher
{
    public string Hash(string rawValue)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawValue));
        return Convert.ToHexString(bytes);
    }
}
