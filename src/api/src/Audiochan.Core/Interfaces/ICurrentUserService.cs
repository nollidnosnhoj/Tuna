namespace Audiochan.Core.Interfaces
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the user id from the claims
        /// </summary>
        /// <returns>user id</returns>
        long GetUserId();
        
        /// <summary>
        /// Tries to get the user id from the claims
        /// </summary>
        /// <param name="userId">A reference to the user id from the claims</param>
        /// <returns>boolean that represents if the user id was in the claims</returns>
        bool TryGetUserId(out long userId);
        
        /// <summary>
        /// Get username from the claims.
        /// </summary>
        /// <returns>username</returns>
        string GetUsername();
        
        /// <summary>
        /// Tries to get username from the claims
        /// </summary>
        /// <param name="username">A reference to the username from the claims.</param>
        /// <returns>boolean the represents if the username was in the claims</returns>
        bool TryGetUsername(out string username);
        
        /// <summary>
        /// Check if the requester is authenticated.
        /// </summary>
        /// <returns>boolean</returns>
        bool IsAuthenticated();
    }
}