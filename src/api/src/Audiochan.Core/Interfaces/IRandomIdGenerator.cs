using System.Threading.Tasks;

namespace Audiochan.Core.Interfaces
{
    public interface IRandomIdGenerator
    {
        public Task<string> GenerateAsync(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
    }
}