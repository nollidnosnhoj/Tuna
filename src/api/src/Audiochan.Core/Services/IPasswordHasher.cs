namespace Audiochan.Core.Services
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="text">Plaintext password</param>
        /// <returns>A hashed password</returns>
        string Hash(string text);
        
        /// <summary>
        /// Verifies that the hash of the given text matches the provided hash.
        /// </summary>
        /// <param name="text">The text to verify.</param>
        /// <param name="hash">The hashed-password</param>
        /// <returns>boolean</returns>
        bool Verify(string text, string hash);
    }
}