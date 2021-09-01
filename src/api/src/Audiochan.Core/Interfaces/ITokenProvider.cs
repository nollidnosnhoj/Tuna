using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITokenProvider
    {
        (string, long) GenerateAccessToken(User user);
        Task<(string, long)> GenerateRefreshToken(User user, string tokenToBeRemoved = "");
        Task<bool> ValidateRefreshToken(string token);
    }
}