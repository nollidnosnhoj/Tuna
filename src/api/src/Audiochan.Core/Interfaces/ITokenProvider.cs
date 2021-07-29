using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITokenProvider
    {
        Task<(string, long)> GenerateAccessToken(User user);
        Task<(string, long)> GenerateRefreshToken(User user, string tokenToBeRemoved = "");
        Task<bool> ValidateRefreshToken(string token);
    }
}