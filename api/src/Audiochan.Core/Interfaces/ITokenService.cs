using System;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(long userId);
        long DateTimeToUnixEpoch(DateTime dateTime);
        bool IsRefreshTokenValid(RefreshToken existingToken);
    }
}