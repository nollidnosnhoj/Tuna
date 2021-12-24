using System.Threading.Tasks;

namespace Audiochan.Core.Commons.Services
{
    public interface IRandomIdGenerator
    {
        public Task<string> GenerateAsync(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
    }
}