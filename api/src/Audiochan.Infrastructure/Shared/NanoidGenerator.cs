using System.Threading.Tasks;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Shared
{
    public class NanoidGenerator : INanoidGenerator
    {
        public string Generate(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            return Nanoid.Nanoid.Generate(chars, size);
        }

        public async Task<string> GenerateAsync(int size = 21, string chars = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            return await Nanoid.Nanoid.GenerateAsync(chars, size);
        }
    }
}