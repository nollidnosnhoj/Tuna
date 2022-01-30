using System.Threading.Tasks;
using Audiochan.Application.Commons.Services;

namespace Audiochan.Infrastructure.Shared
{
    public class NanoidGenerator : IRandomIdGenerator
    {
        public async Task<string> GenerateAsync(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            return await Nanoid.Nanoid.GenerateAsync(chars, size);
        }
    }
}