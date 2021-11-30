using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Security
{
    public class BCryptHasher : IPasswordHasher
    {
        public string Hash(string text)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(text);
        }

        public bool Verify(string text, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(text, hash);
        }
    }
}