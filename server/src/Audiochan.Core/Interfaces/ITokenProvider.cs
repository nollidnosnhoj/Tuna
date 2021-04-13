using System;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITokenProvider
    {
        Task<(string, long)> GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(string userId);
        long DateTimeToUnixEpoch(DateTime dateTime);
    }
}