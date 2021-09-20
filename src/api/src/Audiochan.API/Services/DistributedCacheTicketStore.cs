using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.API.Services
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

            await _cache.SetAsync(key, ticket, options, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            // TODO
            return (await _cache.GetAsync<AuthenticationTicket>(key))!;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}