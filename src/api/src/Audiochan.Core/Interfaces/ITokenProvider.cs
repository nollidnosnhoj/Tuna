using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Generate a JWT access token used to access authorized endpoints.
        /// </summary>
        /// <param name="user">A user entity</param>
        /// <returns>Returns a tuple. First item is the token string. Second item is the expiration date in Unix epoch format.</returns>
        (string, long) GenerateAccessToken(User user);
        
        /// <summary>
        /// Generate a JWT refresh token used to refresh access token once it's expired.
        /// </summary>
        /// <param name="user">User entity. Make sure you loaded the user's refresh tokens.</param>
        /// <param name="tokenToBeRemoved">The user's refresh token that they want to revoke.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A task that returns a tuple when completed.
        /// First item of the tuple is the token string.
        /// Second item is the expiration date in Unix epoch format.</returns>
        Task<(string, long)> GenerateRefreshToken(User user, string tokenToBeRemoved = "", 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Validates the refresh token.
        /// </summary>
        /// <param name="token">The refresh token you want to validate.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A Task that returns a boolean when completed.</returns>
        Task<bool> ValidateRefreshToken(string token, CancellationToken cancellationToken = default);
    }
}