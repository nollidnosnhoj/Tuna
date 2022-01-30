using System.Threading.Tasks;

namespace Audiochan.Application.Commons.Services
{
    public interface IRandomIdGenerator
    {
        public Task<string> GenerateAsync(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
    }
}