namespace Audiochan.Core.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string text);
        bool Verify(string text, string hash);
    }
}