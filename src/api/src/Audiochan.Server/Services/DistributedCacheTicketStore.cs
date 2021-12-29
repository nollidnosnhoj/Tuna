using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Server.Services
{
    public class DistributedCacheTicketStore : ITicketStore
    {
        private readonly IDistributedCache _cache;
        private const string KEY_PREFIX = "session_store-";

        public DistributedCacheTicketStore(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var id = Guid.NewGuid().ToString("N");
            var key = KEY_PREFIX + id;
            await RenewAsync(key, ticket);
            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = ticket.Properties.ExpiresUtc
            };

            var bytes = TicketSerializer.Default.Serialize(ticket);

            await _cache.SetAsync(key, bytes, options);
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var bytes = await _cache.GetAsync(key);
            return bytes is null 
                ? null 
                : TicketSerializer.Default.Deserialize(bytes)!;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}